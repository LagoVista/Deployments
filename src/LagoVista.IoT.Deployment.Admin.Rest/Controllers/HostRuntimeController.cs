using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    public class HostRuntimeController : Controller
    {
        public const string REQUEST_ID = "X-Nuviot-Runtime-Request-Id";
        public const string ORG_ID = "X-Nuviot-Orgid";
        public const string ORG = "X-Nuviot-Org";
        public const string USER_ID = "X-Nuviot-Userid";
        public const string USER = "X-Nuviot-User";
        public const string HOST_ID = "X-Nuviot-Hostid";
        public const string HOST = "X-Nuviot-Host";
        public const string DATE = "X-Nuviot-Date";
        public const string VERSION = "X-Nuviot-Version";

        private readonly IRuntimeTokenManager _runtimeTokenManager;
        private readonly IDeploymentHostManager _hostManager;
        private readonly ISecureStorage _secureStorage;
        private readonly IDeploymentInstanceRepo _instanceRepo;

        private DeploymentHost _host;

        public HostRuntimeController(IDeploymentHostManager hostManager, ISecureStorage secureStorage, IDeploymentInstanceRepo instanceRepo, IRuntimeTokenManager runtimeTokenManager)
        {
            this._runtimeTokenManager = runtimeTokenManager ?? throw new ArgumentNullException(nameof(runtimeTokenManager));
            this._hostManager = hostManager ?? throw new ArgumentNullException(nameof(hostManager));
            this._secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            this._instanceRepo = instanceRepo ?? throw new ArgumentNullException(nameof(instanceRepo));
        }

        private void CheckHeader(HttpRequest request, String header)
        {
            if (!request.Headers.Keys.Contains(header) || String.IsNullOrEmpty(request.Headers[header].ToString()))
            {
                if (!request.Headers.Keys.Contains(header.ToLower()) || String.IsNullOrEmpty(request.Headers[header.ToLower()].ToString()))
                {
                    throw new NotAuthorizedException($"Missing request id header: {header}");
                }
            }
        }

        private string GetHeader(HttpRequest request, String header)
        {
            if (Request.Headers.ContainsKey(header))
            {
                return Request.Headers[header];
            }
            else if (Request.Headers.ContainsKey(header.ToLower()))
            {
                return Request.Headers[header.ToLower()];
            }

            throw new NotAuthorizedException($"Missing request id header: {header}");
        }

        private string GetSignature(string requestId, string key, string source)
        {
            var encData = Encoding.UTF8.GetBytes(source);

            var hmac = new HMac(new Sha256Digest());

            hmac.Init(new KeyParameter(Encoding.UTF8.GetBytes(key)));

            var resultBytes = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(encData, 0, encData.Length);
            hmac.DoFinal(resultBytes, 0);

            var b64Str = System.Convert.ToBase64String(resultBytes);
            return $"SAS {requestId}:{b64Str}";
        }

        protected async Task ValidateRequest(HttpRequest request)
        {
            CheckHeader(request, DATE);
            CheckHeader(request, VERSION);
            CheckHeader(request, REQUEST_ID);
            CheckHeader(request, ORG_ID);
            CheckHeader(request, ORG);
            CheckHeader(request, USER_ID);
            CheckHeader(request, USER);
            CheckHeader(request, HOST_ID);
            CheckHeader(request, HOST);

            var authheader = request.Headers["Authorization"];

            var requestId = GetHeader(request, REQUEST_ID);
            var dateStamp = GetHeader(request, DATE);
            var orgId = GetHeader(request, ORG_ID);
            var org = GetHeader(request, ORG);
            var userId = GetHeader(request, USER_ID);
            var user = GetHeader(request, USER);
            var hostId = GetHeader(request, HOST_ID);
            var hostName = GetHeader(request, HOST);
            var version = GetHeader(request, VERSION);

            var bldr = new StringBuilder();
            //Adding the \r\n manualy ensures that the we don't have any 
            //platform specific code messing with our signature.
            bldr.Append($"{requestId}\r\n");
            bldr.Append($"{dateStamp}\r\n");
            bldr.Append($"{version}\r\n");
            bldr.Append($"{orgId}\r\n");
            bldr.Append($"{userId}\r\n");
            bldr.Append($"{hostId}\r\n");

            OrgEntityHeader = EntityHeader.Create(orgId, org);
            UserEntityHeader = EntityHeader.Create(userId, user);
            InstanceEntityHeader = EntityHeader.Create(hostId, hostName);

            Console.WriteLine($"Loadedinst Host:  {hostId}");
            _host = await _hostManager.GetSecureDeploymentHostAsync(hostId, OrgEntityHeader, UserEntityHeader);
            Console.WriteLine($"Loaded Host:  {_host.Name}");
            
            var calculatedFromFirst = GetSignature(requestId, _host.HostAccessKey1, bldr.ToString());

            if (calculatedFromFirst != authheader)
            {
                var calculatedFromSecond = GetSignature(requestId, _host.HostAccessKey2, bldr.ToString());
                if (calculatedFromSecond != authheader)
                {
                    throw new UnauthorizedAccessException("Invalid signature.");
                }
            }
        }

        protected EntityHeader OrgEntityHeader { get; private set; }
        protected EntityHeader UserEntityHeader { get; private set; }
        protected EntityHeader InstanceEntityHeader { get; private set; }

        [HttpGet("/api/deployment/host")]
        public async Task<InvokeResult<DeploymentHost>> GetInstance()
        {
            await ValidateRequest(Request);
            return InvokeResult<DeploymentHost>.Create(_host);
        }

        [HttpGet("/api/deployment/host/shared/instances")]
        public async Task<InvokeResult<IEnumerable<SharedInstanceSummary>>> GetInstances()
        {
            await ValidateRequest(Request);
            var instances = _host.DeployedInstances;

            foreach (var inst in instances)
            {
                var instance = await _instanceRepo.GetInstanceAsync(inst.Instance.Id);
                var res1 = await _secureStorage.GetSecretAsync(instance.OwnerOrganization, instance.SharedAccessKeySecureId1, UserEntityHeader);
                if (res1.Successful)
                    inst.SharedAccessKey1 = res1.Result;
                else
                    return InvokeResult<IEnumerable<SharedInstanceSummary>>.FromInvokeResult(res1.ToInvokeResult());

                var res2 = await _secureStorage.GetSecretAsync(instance.OwnerOrganization, instance.SharedAccessKeySecureId2, UserEntityHeader);
                if (res2.Successful)
                   inst.SharedAccessKey2 = res2.Result;
                else
                    return InvokeResult<IEnumerable<SharedInstanceSummary>>.FromInvokeResult(res2.ToInvokeResult());
            }

            return InvokeResult<IEnumerable<SharedInstanceSummary>>.Create(instances);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for Web Socket Notify Connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/wsnotify/rabbitmq/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetRabbitMQNotifyConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetRabbitMQWSNotifyConnectionAsync(SettingType.Host, InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for Web Socket Notify Connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/wsnotify/azureeventhub/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetAzureEventHubWSNotifyConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetAzureEventHubsWSNotifyConnectionAsync(SettingType.Host, InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for RPC Service Bus Connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/rpc/settings")]
        public async Task<InvokeResult<RPCSettings>> GetRPCConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetRPCConnectionAsync(SettingType.Host, InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

    }
}
