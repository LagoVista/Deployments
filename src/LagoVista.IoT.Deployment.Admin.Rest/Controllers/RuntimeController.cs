﻿using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models.Settings;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core;
using LagoVista.UserAdmin.Interfaces.Repos.Orgs;
using LagoVista.UserAdmin.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.UserAdmin.Interfaces.Managers;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Deployment.Models;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    public class RuntimeController : Controller
    {
        IDeploymentInstanceManager _instanceManager;
        ISecureStorage _secureStorage;
        IDeploymentHostManager _hostManager;
        IRuntimeTokenManager _runtimeTokenManager;
        IAppUserManagerReadOnly _userManager;
        IOrgUserRepo _orgUserRepo;
        IEmailSender _emailSender;
        ISmsSender _smsSender;
        IServiceTicketCreator _ticketCreator;
        IDistributionManager _distroManager;

        public const string REQUEST_ID = "X-Nuviot-Runtime-Request-Id";
        public const string ORG_ID = "X-Nuviot-Orgid";
        public const string ORG = "X-Nuviot-Org";
        public const string USER_ID = "X-Nuviot-Userid";
        public const string USER = "X-Nuviot-User"; 
        public const string INSTANCE_ID = "X-Nuviot-Instanceid";
        public const string INSTANCE = "X-Nuviot-Instance";
        public const string DATE = "X-Nuviot-Date";
        public const string VERSION = "X-Nuviot-Version";

        public RuntimeController(IDeploymentInstanceManager instanceManager, IRuntimeTokenManager runtimeTokenManager,
            IOrgUserRepo orgUserRepo, IAppUserManagerReadOnly userManager, IDeploymentHostManager hostManager,
            IServiceTicketCreator ticketCreator, IEmailSender emailSender, ISmsSender smsSendeer,
            IDistributionManager distroManager, ISecureStorage secureStorage, IAdminLogger logger)
        {
            _ticketCreator = ticketCreator ?? throw new ArgumentNullException(nameof(ticketCreator));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _orgUserRepo = orgUserRepo ?? throw new ArgumentNullException(nameof(orgUserRepo));
            _instanceManager = instanceManager ?? throw new ArgumentNullException(nameof(instanceManager));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _runtimeTokenManager = runtimeTokenManager ?? throw new ArgumentNullException(nameof(runtimeTokenManager));
            _hostManager = hostManager ?? throw new ArgumentNullException(nameof(hostManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _distroManager = distroManager ?? throw new ArgumentNullException(nameof(distroManager));
            _smsSender = smsSendeer ?? throw new ArgumentNullException(nameof(smsSendeer));
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
            if(Request.Headers.ContainsKey(header))
            {
                return Request.Headers[header];
            }
            else if(Request.Headers.ContainsKey(header.ToLower()))
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
            CheckHeader(request, INSTANCE_ID);
            CheckHeader(request, INSTANCE);                        

            var authheader = request.Headers["Authorization"];

            var requestId = GetHeader(request, REQUEST_ID);
            var dateStamp = GetHeader(request, DATE);
            var orgId = GetHeader(request, ORG_ID);
            var org = GetHeader(request, ORG);
            var userId = GetHeader(request, USER_ID);
            var user = GetHeader(request, USER);
            var instanceId = GetHeader(request, INSTANCE_ID);
            var instanceName = GetHeader(request, INSTANCE);
            var version = GetHeader(request, VERSION);

            var bldr = new StringBuilder();
            //Adding the \r\n manualy ensures that the we don't have any 
            //platform specific code messing with our signature.
            bldr.Append($"{requestId}\r\n");
            bldr.Append($"{dateStamp}\r\n");
            bldr.Append($"{version}\r\n");
            bldr.Append($"{orgId}\r\n");
            bldr.Append($"{userId}\r\n");
            bldr.Append($"{instanceId}\r\n");

            OrgEntityHeader = EntityHeader.Create(orgId, org);
            UserEntityHeader = EntityHeader.Create(userId, user);
            InstanceEntityHeader = EntityHeader.Create(instanceId, instanceName);

            var instance = await _instanceManager.GetInstanceAsync(instanceId, OrgEntityHeader, UserEntityHeader);
            var key1 = await _secureStorage.GetSecretAsync(OrgEntityHeader, instance.SharedAccessKeySecureId1, UserEntityHeader);
            if (!key1.Successful)
            {
                throw new Exception(key1.Errors.First().Message);
            }

            var calculatedFromFirst = GetSignature(requestId, key1.Result, bldr.ToString());

            if (calculatedFromFirst != authheader)
            {
                var key2 = await _secureStorage.GetSecretAsync(OrgEntityHeader, instance.SharedAccessKeySecureId2, UserEntityHeader);
                if (!key2.Successful)
                {
                    throw new Exception(key2.Errors.First().Message);
                }

                var calculatedFromSecond = GetSignature(requestId, key2.Result, bldr.ToString());
                if (calculatedFromSecond != authheader)
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

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Newtonsoft.Json.Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        /// <summary>
        /// Deployment Instance - Get Full
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/full")]
        public async Task<string> GetFullInstanceAsync()
        {
            await ValidateRequest(HttpContext.Request);
            var deviceInstanceResponse = await _instanceManager.LoadFullInstanceAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
            return JsonConvert.SerializeObject(deviceInstanceResponse, _jsonSettings);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for device watchdog timer
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/devicewatchdog/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetDeviceWatchdogConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetDeviceWatchdogConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for device event connection storage
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/deviceconnectionevent/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetDeviceEventConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetDeviceEventConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
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
            return await _runtimeTokenManager.GetAzureEventHubsWSNotifyConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for Web Socket Notify Connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/wsnotify/azureeventhub/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetAzureEventHubWSNotifyConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetAzureEventHubsWSNotifyConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Request Connection for Web Socket Notify Connection
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/wsnotify/rabbitmq/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetRabbitMQNotifyConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetRabbitMQWSNotifyConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
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
        /// Runtime Controller - Get Storage Settings for EH Check Point
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/ehcheckpoint/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetEHCheckPointSettingsAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetEHCheckPointStorageSttings(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get User by User Id, user must belong to the org that has the deployment that is requesting the data, no sensitive data will be sent.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/usernotifinfo/{id}")]
        public async Task<InvokeResult<UserNotificationInfo>> GetUserAsync(string id)
        {
            await ValidateRequest(HttpContext.Request);

            if (await _orgUserRepo.QueryOrgHasUserAsync(OrgEntityHeader.Id, id))
            {
                var user = await _userManager.GetUserByIdAsync(id, OrgEntityHeader, UserEntityHeader);
                if(user == null)
                {
                    return InvokeResult<UserNotificationInfo>.FromError("Could not find requested user by user id");
                }
                else
                {
                    return InvokeResult<UserNotificationInfo>.Create(new UserNotificationInfo()
                    {
                        Id = id,
                        Email = user.Email.ToLower(),
                        Phone = user.PhoneNumber
                    });
                }
            }
            else
            {
                return InvokeResult<UserNotificationInfo>.FromError("Requested user does not belong to deployment org");
            }
        }

        private async Task<InvokeResult> SendToUserAsync(UserMessage message)
        {
            if (!String.IsNullOrEmpty(message.UserId))
            {
                var getUserResult = await GetUserAsync(message.UserId);
                if (!getUserResult.Successful)
                {
                    throw new InvalidDataException("RuntimeController_SendUserMessageAsync", getUserResult.Errors.Select(er => er.Message).ToArray());
                }

                if (String.IsNullOrEmpty(message.Phone))
                {
                    message.Phone = getUserResult.Result.Phone;
                }

                if (String.IsNullOrEmpty(message.Email))
                {
                    message.Email = getUserResult.Result.Email;
                }
            }

            if (String.IsNullOrEmpty(message.Phone) && (message.MessageType == MessageTypes.SMS || message.MessageType == MessageTypes.SMSAndEmail))
            {
                if (String.IsNullOrEmpty(message.UserId))
                    throw new InvalidDataException(errors: new string[] { "Did not supply User Id to load user, and phone number was not provided." });
                else
                    throw new InvalidDataException(errors: new string[] { $"User with id: {message.UserId} does not have a phone number." });
            }

            if (String.IsNullOrEmpty(message.Email) && (message.MessageType == MessageTypes.Email || message.MessageType == MessageTypes.SMSAndEmail))
            {
                if (String.IsNullOrEmpty(message.UserId))
                    throw new InvalidDataException(errors: new string[] { "Did not supply User Id to load user, and email  was not provided." });
                else
                    throw new InvalidDataException(errors: new string[] { $"User with id: {message.UserId} does not have a valid email address." });
            }

            switch (message.MessageType)
            {
                case MessageTypes.Email:
                    if (String.IsNullOrEmpty(message.Body) || String.IsNullOrEmpty(message.Subject))
                    {
                        throw new InvalidDataException(errors: new string[] { "Body and Subject are required to send an email." });
                    }

                    await _emailSender.SendAsync(message.Email, message.Subject, message.Body);
                    break;
                case MessageTypes.SMS:
                    if (String.IsNullOrEmpty(message.Body))
                    {
                        throw new InvalidDataException(errors: new string[] { "Body is required to send an SMS Message." });
                    }

                    await _smsSender.SendAsync(message.Phone, message.Body);

                    break;
                case MessageTypes.SMSAndEmail:
                    if (String.IsNullOrEmpty(message.Body) || String.IsNullOrEmpty(message.Subject))
                    {
                        throw new InvalidDataException(errors: new string[] { "Body and Subject are required." });
                    }

                    await _smsSender.SendAsync(message.Phone, message.Body);
                    await _emailSender.SendAsync(message.Email, message.Subject, message.Body);
                    break;
            }

            return InvokeResult.Success;
        }

        [HttpPost("/api/deployment/user/message")]
        public async Task<InvokeResult> SendUserNotificationAsync([FromBody] UserMessage message)
        {
            await ValidateRequest(HttpContext.Request);

            if(!String.IsNullOrEmpty(message.UserId))
            {
                return await SendToUserAsync(message);
            }

            var group = await _distroManager.GetListAsync(message.DistributionGroupId, OrgEntityHeader, UserEntityHeader);
            if (group == null) return InvokeResult.FromError($"Could not load distribution group for group id [{message.DistributionGroupId}].");

            foreach(var user in group.AppUsers)
            {
                message.UserId = user.Id;
                // before calling the method to send an email to a user
                // clear the phone/email on th emessage this will allow
                // the email message to send to the user id, not just the 
                // email on the message
                message.Phone = null;
                message.Email = null;
                var sendResult = await SendToUserAsync(message);
                if (!sendResult.Successful) return sendResult;
            }

            return InvokeResult.Success;
        }

        /// <summary>
        /// Runtime Controller - Create a Service Ticket
        /// </summary>
        /// <param name="templateid">Template Id of Ticket to Create</param>
        /// <param name="repoid">Device Repository for the device that the ticket will be creatd for.</param>
        /// <param name="deviceid">Device Id (32 byte unique id) of the device for the ticket </param>
        /// <returns></returns>
        [HttpGet("/api/deployment/fslite/ticket/{templateid}/{repoid}/{deviceid}")]
        public Task<InvokeResult<string>> CreateTicket(string templateid, string repoid, string deviceid)
        {
            return _ticketCreator.CreateServiceTicketAsync(templateid, repoid, deviceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Create a Service Ticket
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/deployment/fslite/ticket")]
        public Task<InvokeResult<string>> CreateTicketAsync([FromBody] CreateServiceTicketRequest request)
        {
            return _ticketCreator.CreateServiceTicketAsync(request,  OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get PEM Storage Connection 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/pem/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetPEMStorageConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetPEMStorageSettingsAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get Device Storage Settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/devicestorage/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetDeviceStorageAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetDeviceStorageConnectionAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);

        }

        /// <summary>
        /// Runtime Controller - Get Device Data Settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/devicedata/settings")]
        public async Task<InvokeResult<DeviceDataStorageSettings>> GetDeviceArchiveStorageConnectionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetDataStorageSettingsAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get Logging Settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/logging/settings")]
        public async Task<InvokeResult<LoggingSettings>> GetLoggingSettings()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetLoggingSettingsAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get Cache Settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/cache/settings")]
        public async Task<InvokeResult<ConnectionSettings>> GetCacheSettings()
        {
            await ValidateRequest(HttpContext.Request);
            return await _runtimeTokenManager.GetNuvIoTCacheSettingsAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Runtime Controller - Get Instance Solution Versionn 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/solutionversion")]
        public async Task<InvokeResult<string>> GetSolutionVersionAsync()
        {
            await ValidateRequest(HttpContext.Request);
            var instance = await _instanceManager.GetInstanceAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);
            if (EntityHeader.IsNullOrEmpty(instance.Version))
            {
                return InvokeResult<string>.Create("latest");
            }
            else
            {
                return InvokeResult<string>.Create(instance.Version.Id);
            }
        }

        /// <summary>
        /// Runtime Controller - Get Instance Solution Versionn 
        /// </summary>
        /// <param name="hostid">ID Of the Host </param>
        /// <param name="status">true/false</param>
        /// <param name="version">Current version of runtime.</param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{hostid}/{status}/{version}")]
        public async Task<InvokeResult> UpdateHostStatusAsync(string hostid, string status, string version)
        {
            if (String.IsNullOrEmpty(hostid)) return InvokeResult.FromError("host id is a required field.");

            await ValidateRequest(HttpContext.Request);
            var instance = await _instanceManager.GetInstanceAsync(InstanceEntityHeader.Id, OrgEntityHeader, UserEntityHeader);

            if (Enum.TryParse<HostStatus>(status, out HostStatus hostStatus))
            {
                return await _hostManager.UpdateDeploymentHostStatusAsync(hostid, hostStatus, version, OrgEntityHeader, UserEntityHeader);
            }
            else
            {
                return InvokeResult.FromError($"Could not parse [status] to DeploymentInstanceStates");
            }
        }

        /// <summary>
        /// Runtime Controller - pdate Status Async
        /// </summary>
        /// <param name="status">New Stauts must be one of DeploymentInstanceStates </param>
        /// <param name="isdeployed">true/false</param>
        /// <param name="version">Current version of runtime.</param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/status/{status}/{isdeployed}/{version}")]
        public async Task<InvokeResult> UpdateInstanceStatusAsync(String status, bool isdeployed, string version)
        {
            await ValidateRequest(HttpContext.Request);
            if (Enum.TryParse<DeploymentInstanceStates>(status, out DeploymentInstanceStates newStatus))
            {
                return await _instanceManager.UpdateInstanceStatusAsync(InstanceEntityHeader.Id, newStatus, isdeployed, version, OrgEntityHeader, UserEntityHeader);
            }
            else
            {
                return InvokeResult.FromError($"Could not parse [status] to DeploymentInstanceStates");
            }

        }

        /// <summary>
        /// Handle a device exception.
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/device/handlexception")]
        public async Task<InvokeResult> HandleDeviceExcptionAsync([FromBody] DeviceException deviceExcpetion)
        {
            await ValidateRequest(HttpContext.Request);

            return await _ticketCreator.HandleDeviceExceptionAsync(deviceExcpetion, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Clear a device exception.
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/device/clearexception")]
        public async Task<InvokeResult> ClearDeviceExcptionAsync([FromBody] DeviceException deviceExcpetion)
        {
            await ValidateRequest(HttpContext.Request);

            return await _ticketCreator.ClearDeviceExceptionAsync(deviceExcpetion, OrgEntityHeader, UserEntityHeader);
        }
    }
}
