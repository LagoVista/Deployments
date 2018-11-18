using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System.Linq;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    public class RuntimeController : Controller
    {

        IDeploymentInstanceManager _instanceManager;
        IDeviceRepoTokenBroker _deviceRepoTokenBroker;
        ISecureStorage _secureStorage;

        public const string REQUEST_ID = "x-nuviot-runtime-request-id";
        public const string ORG_ID = "x-nuviot-orgid";
        public const string ORG = "x-nuviot-org";
        public const string USER_ID = "x-nuviot-userid";
        public const string USER = "x-nuviot-user";
        public const string INSTANCE_ID = "x-nuviot-instanceid";
        public const string INSTANCE = "x-nuviot-instance";
        public const string DATE = "x-nuviot-date";
        public const string VERSION = "x-nuviot-version";

        public RuntimeController(IDeploymentInstanceManager instanceManager, IDeviceRepoTokenBroker tokenBroker, ISecureStorage secureStorage, IAdminLogger logger)
        {
            _instanceManager = instanceManager;
            _deviceRepoTokenBroker = tokenBroker;
            _secureStorage = secureStorage;
        }

        private void CheckHeader(HttpRequest request, String header)
        {
            if (!request.Headers.Keys.Contains(header))
            {
                throw new NotAuthorizedException($"Missing request id header: {header}");
            }
        }

        protected async Task ValidateRequest(HttpRequest request)
        {
            CheckHeader(request, REQUEST_ID);
            CheckHeader(request, ORG_ID);
            CheckHeader(request, ORG);
            CheckHeader(request, USER_ID);
            CheckHeader(request, USER);
            CheckHeader(request, INSTANCE_ID);
            CheckHeader(request, INSTANCE);
            CheckHeader(request, DATE);
            CheckHeader(request, VERSION);
     
            var authheader = request.Headers["Authorization"];

            var requestId = request.Headers[REQUEST_ID];
            var dateStamp = request.Headers[DATE];
            var orgId = request.Headers[ORG_ID];
            var org = request.Headers[ORG];
            var userId = request.Headers[USER_ID];
            var user = request.Headers[USER];
            var instanceId = request.Headers[INSTANCE_ID];
            var instanceName = request.Headers[INSTANCE];
            var version = request.Headers[VERSION];

            var bldr = new StringBuilder();
            bldr.AppendLine(requestId);
            bldr.AppendLine(dateStamp);
            bldr.AppendLine(version);
            bldr.AppendLine(orgId);
            bldr.AppendLine(userId);
            bldr.AppendLine(instanceId);

            Console.Write(bldr.ToString());

            var encData = Encoding.UTF8.GetBytes(bldr.ToString());

            OrgEntityHeader = EntityHeader.Create(orgId, org);
            UserEntityHeader = EntityHeader.Create(userId, user);
            InstanceEntityHeader = EntityHeader.Create(instanceId, instanceName);

            var instance = await _instanceManager.GetInstanceAsync(instanceId, OrgEntityHeader, UserEntityHeader);
            var key1 = await _secureStorage.GetSecretAsync(OrgEntityHeader, instance.SharedAccessKeySecureId1, UserEntityHeader);
        
            if(!key1.Successful)
            {
                throw new Exception(key1.Errors.First().Message);
            }

            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(Encoding.UTF8.GetBytes(key1.Result)));
            Console.WriteLine(key1.Result);

            var resultBytes = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(encData, 0, encData.Length);
            hmac.DoFinal(resultBytes, 0);

            var b64Str = System.Convert.ToBase64String(resultBytes);
            var sharedSignature = $"SAS {requestId}:{b64Str}";

            Console.WriteLine(sharedSignature);
            Console.WriteLine(authheader);
        }

        protected EntityHeader OrgEntityHeader { get; private set; }
        protected EntityHeader UserEntityHeader { get; private set; }
        protected EntityHeader InstanceEntityHeader { get; private set; }

        /// <summary>
        /// Runtime Controller - Request a key for a run time.
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/key/{keyid}")]
        public async Task<InvokeResult<string>> RequestKeyAsync(string keyid)
        {
            await ValidateRequest(HttpContext.Request);
            return await _instanceManager.GetKeyAsync(keyid, InstanceEntityHeader, OrgEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request a token to use NuvIoT repo.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/devicerepo/token")]
        public async Task<InvokeResult<DeviceRepoToken>> GetDeviceRepoTokenAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _deviceRepoTokenBroker.GetDeviceRepoTokenAsync(InstanceEntityHeader, OrgEntityHeader);
        }
    }
}
