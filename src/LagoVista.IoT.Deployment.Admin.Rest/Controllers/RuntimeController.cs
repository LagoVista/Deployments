using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Models.Settings;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    public class RuntimeController : Controller
    {

        IDeploymentInstanceManager _instanceManager;
        ISecureStorage _secureStorage;
        IRuntimeTokenManager _runtimeTokenManager;

        public const string REQUEST_ID = "x-nuviot-runtime-request-id";
        public const string ORG_ID = "x-nuviot-orgid";
        public const string ORG = "x-nuviot-org";
        public const string USER_ID = "x-nuviot-userid";
        public const string USER = "x-nuviot-user";
        public const string INSTANCE_ID = "x-nuviot-instanceid";
        public const string INSTANCE = "x-nuviot-instance";
        public const string DATE = "x-nuviot-date";
        public const string VERSION = "x-nuviot-version";

        public RuntimeController(IDeploymentInstanceManager instanceManager,IRuntimeTokenManager runtimeTokenManager,
            ISecureStorage secureStorage, IAdminLogger logger)
        {
            _instanceManager = instanceManager;
            _secureStorage = secureStorage;
            _runtimeTokenManager = runtimeTokenManager;
        }

        private void CheckHeader(HttpRequest request, String header)
        {
            if (!request.Headers.Keys.Contains(header))
            {
                throw new NotAuthorizedException($"Missing request id header: {header}");
            }
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


            OrgEntityHeader = EntityHeader.Create(orgId, org);
            UserEntityHeader = EntityHeader.Create(userId, user);
            InstanceEntityHeader = EntityHeader.Create(instanceId, instanceName);

            var instance = await _instanceManager.GetInstanceAsync(instanceId, OrgEntityHeader, UserEntityHeader);
            var key1 = await _secureStorage.GetSecretAsync(OrgEntityHeader, instance.SharedAccessKeySecureId1, UserEntityHeader);
            if(!key1.Successful) throw new Exception(key1.Errors.First().Message);

            var calculatedFromFirst = GetSignature(requestId, key1.Result, bldr.ToString());

            if(calculatedFromFirst != authheader)
            {
                var key2 = await _secureStorage.GetSecretAsync(OrgEntityHeader, instance.SharedAccessKeySecureId2, UserEntityHeader);
                if (!key2.Successful) throw new Exception(key1.Errors.First().Message);
                var calculatedFromSecond = GetSignature(requestId, key2.Result, bldr.ToString());
                if(calculatedFromSecond != authheader)
                {
                    throw new UnauthorizedAccessException("Invalid signature.");
                }
            }
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
        /// Deployment Instance - Get Full
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/full")]
        public async Task<InvokeResult<DeploymentInstance>> GetFullInstanceAsync(String id)
        {
            await ValidateRequest(HttpContext.Request);
            var deviceInstance = await _instanceManager.LoadFullInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
            if (deviceInstance.Successful)
            {
                return InvokeResult<DeploymentInstance>.Create(deviceInstance.Result);
            }
            else
            {
                var resp = InvokeResult<DeploymentInstance>.Create(null);
                resp.Errors.AddRange(deviceInstance.Errors);
                return resp;
            }
        }

        /// <summary>
        /// Runtime Controller - Request Connection for Usage Storage Storage
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/usage/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetUsageStorageConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetUsageStorageConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for Web Socket Notify Connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/wsnotify/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetWSNotifyConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetWSNotifyConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for RPC Service Bus Connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/rpc/settings")]
        public async Task<InvokeResult<RPCSettings>> GetRPCConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetRPCConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get PEM Storage Connection 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/pem/settings")]
        public async Task<InvokeResult<PEMStorageSettings>> GetPEMStorageConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetPEMStorageSettingsAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get PEM Storage Connection 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/devicestorage/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetDeviceStorageAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetDeviceStorageConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);

        }

        /// <summary>
        /// Runtime Controller - Get PEM Storage Connection 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/devicedata/settings")]
        public async Task<InvokeResult<DeviceDataStorageSettings>> GetDeviceArchiveStorageConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetDataStorageSettingsAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);

        }
    }
}
