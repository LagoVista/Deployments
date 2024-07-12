using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Pipeline.Admin.Models;
using LagoVista.IoT.Pipeline.Models;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{

    /// <summary>
    /// Manage Deployment Instances 
    /// </summary>
    [ConfirmedUser]
    [Authorize]
    public class DeploymentInstanceController : LagoVistaBaseController
    {
        private readonly IDeploymentInstanceManager _instanceManager;
        private readonly ITimeZoneServices _timeZoneServices;
        private readonly IContainerRepositoryManager _containerRepoManager;

        public DeploymentInstanceController(IDeploymentInstanceManager instanceManager, ITimeZoneServices timeZoneServices, IContainerRepositoryManager containerRepoManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _instanceManager = instanceManager ?? throw new ArgumentNullException(nameof(instanceManager)); ;
            _timeZoneServices = timeZoneServices ?? throw new ArgumentNullException(nameof(timeZoneServices));
            _containerRepoManager = containerRepoManager ?? throw new ArgumentNullException(nameof(containerRepoManager));
        }

        /// <summary>
        /// Deployment Instance - Add
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/instance")]
        public Task<InvokeResult> AddInstanceAsync([FromBody] DeploymentInstance instance)
        {
            return _instanceManager.AddInstanceAsync(instance, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Add shared instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/host/{id}/add/{instanceid}")]
        public Task<InvokeResult> AddShardHost(string id, string instanceid)
        {
            return _instanceManager.AddToShardHostAsync(id, instanceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Remove a shared instance from shared host.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        [HttpDelete("/api/deployment/host/{id}/remove/{instanceid}")]
        public Task<InvokeResult> RemoveSharedInstance(string id, string instanceid)
        {
            return _instanceManager.RemoveSharedInstanceAsync(id, instanceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Update
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [HttpPut("/api/deployment/instance")]
        public Task<InvokeResult> UpdateInstanceAsync([FromBody] DeploymentInstance instance)
        {
            SetUpdatedProperties(instance);
            return _instanceManager.UpdateInstanceAsync(instance, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get for Org
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instances")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync()
        {
            return await _instanceManager.GetInstanceForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/instances")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetAllInstancesAsync()
        {
            var listRsponse = await _instanceManager.SysAdminGetAllInstancesAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
            listRsponse.GetListUrl = "/sys/api/deployment/instances";
            return listRsponse;
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/instances/{orgid}")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetAllInstancesAsync(string orgid)
        {
            var listRsponse = await _instanceManager.SysAdminGetInstancesAsync(orgid, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
            listRsponse.GetListUrl = "/sys/api/deployment/instances/{id}";
            return listRsponse;
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/instances/active")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetAllActiveInstancesAsync()
        {
            var listRsponse = await _instanceManager.SysAdminGetActiveInstancesAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
            listRsponse.GetListUrl = "/sys/api/deployment/instances/active";
            return listRsponse;
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/instances/failed")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetFailedInstancesAsync()
        {
            var listRsponse = await _instanceManager.SysAdminFailedInstancesAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
            listRsponse.GetListUrl = "/sys/api/deployment/instances/failed";
            return listRsponse;
        }

        /// <summary>
        /// Deployment Instance - Get for Org
        /// </summary>
        /// <param name="str" />
        /// <returns></returns>
        [HttpGet("/api/deployment/instances/{str}")]
        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync(string str)
        {
            if (Enum.TryParse<NuvIoTEditions>(str, out NuvIoTEditions edition))
            {
                return await _instanceManager.GetInstanceForOrgAsync(edition, OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
            }

            throw new InvalidOperationException($"{str} is not a valid edition");
        }

        /// <summary>
        /// Deployment Instance - Get Status History
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/statushistory")]
        public Task<ListResponse<DeploymentInstanceStatus>> GetDeploymentInstanceStatusHistoryAsync(string id)
        {
            return _instanceManager.GetDeploymentInstanceStatusHistoryAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Deployment Instance - Get Status History
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{instanceid}/device/{id}/notifications/silence")]
        public Task<InvokeResult> SilenceNotificationAsync(string instanceid, string id)
        {
            return _instanceManager.SetSilenceAlarmAsync(instanceid, id, true, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get Status History
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{instanceid}/device/{id}/notifications/enable")]
        public Task<InvokeResult> EnableNotificationAsync(string instanceid, string id)
        {
            return _instanceManager.SetSilenceAlarmAsync(instanceid, id, false, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Deployment Instance - Check in Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/inuse")]
        public Task<DependentObjectCheckResult> InUseCheck(String id)
        {
            return _instanceManager.CheckInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}")]
        public async Task<DetailResponse<DeploymentInstance>> GetInstanceAsync(String id)
        {
            var deviceInstance = await _instanceManager.GetInstanceAsync(id, OrgEntityHeader, UserEntityHeader);

            var response = DetailResponse<DeploymentInstance>.Create(deviceInstance.Result);
            response.View["timeZone"].Options = _timeZoneServices.GetTimeZones().Select(tz => new EnumDescription() { Key = tz.Id, Label = tz.DisplayName, Name = tz.DisplayName }).ToList();

            return response;
        }

        /// <summary>
        /// Deployment Instance - Is In Test Mode
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/testmode")]
        public Task<InvokeResult<bool>> IsInTestMode(String id)
        {
            return _instanceManager.DeploymentInTestMode(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/debugmode")]
        public Task<InvokeResult<bool>> IsInDebugMode(String id)
        {
            return _instanceManager.DeploymentInDebugMode(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get
        /// </summary>
        /// <param name="id"></param>
        /// <param name="testmode"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/testmode/{testmode}")]
        public async Task<InvokeResult> GetInstanceAsync(String id, bool testmode)
        {
            var deviceInstance = await _instanceManager.GetInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
            deviceInstance.Result.TestMode = testmode;
            await  _instanceManager.UpdateInstanceAsync(deviceInstance.Result, OrgEntityHeader, UserEntityHeader);
            return InvokeResult.Success;
        }


        /// <summary>
        /// Deployment Instance - Get default listener with any required passwords
        /// </summary>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{instanceid}/defaultlistener")]
        public async Task<InvokeResult<ListenerConfiguration>> GetDefaultListenerConfigAsync(String instanceid)
        {
            return await _instanceManager.GetDefaultListenerConfigurationAsync(instanceid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get default listener by device repository with any required passwords
        /// </summary>
        /// <param name="repoid"></param>
        /// <returns></returns>
        [HttpGet("/api/device/repo/{repoid}/defaultlistener")]
        public async Task<InvokeResult<ListenerConfiguration>> GetDefaultListenerConfigForRepoAsync(String repoid)
        {
            return await _instanceManager.GetDefaultListenerConfigurationForRepoAsync(repoid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Get device types that are associated with the devices configurations in the instance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/devicetypes")]
        public async Task<ListResponse<DeviceTypeSummary>> GetDeviceTypesForInstanceAsync(String id)
        {
            return await _instanceManager.GetDeviceTypesForInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Update Status
        /// </summary>
        /// <returns></returns>
        [HttpPut("/api/deployment/instance/status")]
        public Task<InvokeResult> UpdateInstanceStatus([FromBody] InstanceStatusUpdate statusUpdate)
        {
            return _instanceManager.UpdateInstanceStatusAsync(statusUpdate.Id, statusUpdate.NewStatus, statusUpdate.Deployed, statusUpdate.Version,
                OrgEntityHeader, UserEntityHeader, statusUpdate.Details);
        }

        /// <summary>
        /// Deployment Instance - Get Runtime Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/runtime")]
        public Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceRunTimeAsync(String id)
        {
            return _instanceManager.GetInstanceDetailsAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Get connected devices that have been monitored.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/connected/monitored")]
        public Task<ListResponse<DeviceStatus>> GetTimedoutDevicesAsync(string id)
        {
            return _instanceManager.GetWatchdogConnectedDevicesAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Get connected devices that have timed out.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/connected/timedout")]
        public Task<ListResponse<DeviceStatus>> GetWatchdogConnectedDevicesAsync(string id)
        {
            return _instanceManager.GetTimedoutDevicesAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Get all messages watch dogs as monitored by an instance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/message/watchdog/monitored")]
        public Task<ListResponse<WatchdogMessageStatus>> GetWatchdogMessageStatusAsync(string id)
        {
            return _instanceManager.GetWatchdogMessageStatusAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Get all messages that have timed out as monitored by an instance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/message/watchdog/timedout")]
        public Task<ListResponse<WatchdogMessageStatus>> GetTimedOutWatchdogMessageStatusAsync(string id)
        {
            return _instanceManager.GetTimedOutWatchdogMessageStatusAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Device Message Config - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{key}/keyinuse")]
        public Task<bool> InstanceKeyInUse(String key)
        {
            return _instanceManager.QueryInstanceKeyInUseAsync(key, OrgEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/deployment/instance/{id}")]
        public Task<InvokeResult> DeleteInstanceAsync(string id)
        {
            return _instanceManager.DeleteInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Deployment Instance - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/factory")]
        public async Task<DetailResponse<DeploymentInstance>> CreateInstance()
        {
            var response = DetailResponse<DeploymentInstance>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            response.Model.DeploymentConfiguration = EntityHeader<DeploymentConfigurations>.Create(DeploymentConfigurations.SingleInstance);
            response.Model.NuvIoTEdition = EntityHeader<NuvIoTEditions>.Create(NuvIoTEditions.Container);
            response.Model.LogStorage = EntityHeader<LogStorage>.Create(LogStorage.Cloud);
            response.Model.WorkingStorage = EntityHeader<WorkingStorage>.Create(WorkingStorage.Cloud);
            response.Model.DeploymentType = EntityHeader<DeploymentTypes>.Create(DeploymentTypes.Managed);
            response.Model.QueueType = EntityHeader<QueueTypes>.Create(QueueTypes.InMemory);
            response.Model.LogStorage = EntityHeader<LogStorage>.Create(LogStorage.Cloud);
            response.Model.PrimaryCacheType = EntityHeader<CacheTypes>.Create(CacheTypes.LocalInMemory);
            response.Model.SharedAccessKey1 = _instanceManager.GenerateAccessKey();
            response.Model.SharedAccessKey2 = _instanceManager.GenerateAccessKey();
            response.View["timeZone"].Options = _timeZoneServices.GetTimeZones().Select(tz => new EnumDescription() { Key = tz.Id, Label = tz.DisplayName, Name = tz.DisplayName }).ToList();

            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            var repo = await _containerRepoManager.GetDefaultForRuntimeRepoAsync(OrgEntityHeader, UserEntityHeader);
            response.Model.ContainerRepository = repo.ToEntityHeader();
            response.Model.ContainerTag = repo.PreferredTag;
            return response;
        }

        /* Methods to manage the instance */

        /// <summary>
        /// Deployment Instance - Deploy
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/deployhost")]
        public Task<InvokeResult> DeployHostAsync(String id)
        {
            return _instanceManager.DeployHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Start
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/start")]
        public Task<InvokeResult> StartAsync(String id)
        {
            return _instanceManager.StartAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Reset
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/reset")]
        public Task<InvokeResult> ResetAsync(String id)
        {
            return _instanceManager.ResetAppAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Start
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/pause")]
        public Task<InvokeResult> PauseAsync(String id)
        {
            return _instanceManager.PauseAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Reload Solution
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/reloadsolution")]
        public Task<InvokeResult> RealodSolutionAsync(String id)
        {
            return _instanceManager.ReloadSolutionAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Update Runtime
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/updateruntime")]
        public Task<InvokeResult> UpdateRuntimeAsync(String id)
        {
            return _instanceManager.UpdateRuntimeAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Restart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/restarthost")]
        public Task<InvokeResult> RestartHostAsync(String id)
        {
            return _instanceManager.RestartHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Reset Container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/resetartcontainer")]
        public Task<InvokeResult> ResetContainerAsync(String id)
        {
            return _instanceManager.RestartContainerAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Set Docker Image
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="repoid"></param>
        /// <param name="tagid"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{instanceid}/image/{repoid}/{tagid}")]
        public Task<InvokeResult> SetDockerImageAsync(string instanceid, String repoid, string tagid)
        {
            return _instanceManager.SetContainerRepoAsync(instanceid, repoid, tagid, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Stop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/stop")]
        public Task<InvokeResult> StopAsync(String id)
        {
            return _instanceManager.StopAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        [HttpPut("/api/deployment/instance/{id}/wifiprofile")]
        public async Task<InvokeResult<DeploymentInstance>> UpdateWiFiProfile(string id, [FromBody] WiFiConnectionProfile connectionProfile)
        {
            var instance = await _instanceManager.GetInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
            var existing = instance.Result.WiFiConnectionProfiles.FirstOrDefault(ins => ins.Id == connectionProfile.Id);
            if (existing != null)
            {
                instance.Result.WiFiConnectionProfiles.Remove(existing);
                Console.WriteLine("REmove old");
            }

            Console.WriteLine($"Count {instance.Result.WiFiConnectionProfiles.Count}  {connectionProfile.Name} {connectionProfile.Key}");

            instance.Result.WiFiConnectionProfiles.Add(connectionProfile);

            var result = await _instanceManager.UpdateInstanceAsync(instance.Result, OrgEntityHeader, UserEntityHeader);
            if (result.Successful)
                return InvokeResult<DeploymentInstance>.Create(instance.Result);

            return InvokeResult<DeploymentInstance>.FromInvokeResult(result);
        }

        [HttpPost("/api/deployment/instance/{id}/wifiprofile")]
        public Task<InvokeResult<DeploymentInstance>> AddWiFiProfile(string id, [FromBody] WiFiConnectionProfile connectionProfile)
        {
            return UpdateWiFiProfile(id, connectionProfile);
        }

        [HttpPost("/api/deployment/instance/{id}/testmode/{enabled}")]
        public async Task<InvokeResult> SetTaskMode(string id, bool enabled)
        {
            var instance = await _instanceManager.GetInstanceAsync(id, OrgEntityHeader, UserEntityHeader);
            instance.Result.TestMode = enabled;
            return await _instanceManager.UpdateInstanceAsync(instance.Result, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// WiFi Connection Profile - Get
        /// </summary>
        /// <param name="id">Instance Id</param>
        /// <param name="wifiid">WiFi Connection Profile Id</param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/wifiprofile/{wifiid}")]
        public Task<InvokeResult<WiFiConnectionProfile>> GetWiFiConnectionProfile(string id, string wifiid)
        {
            return _instanceManager.GetWiFiConnectionProfileAsync(id, wifiid, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// WiFi Connection Profile - Create
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/wificonnectionprofile/factory")]
        public DetailResponse<WiFiConnectionProfile> CreateWiFiConnectionProfile()
        {
            return DetailResponse<WiFiConnectionProfile>.Create();
        }

        /// <summary>
        /// WiFi Connection Profiles - Get by repo
        /// </summary>
        /// <param name="id">Device Repoitory Id</param>
        /// <returns></returns>
        [HttpGet("/api/device/repo/{id}/wifiprofiles")]
        public Task<List<WiFiConnectionProfile>> GetWiFiConnectionProfiles(string id)
        {
            return _instanceManager.GetWiFiConnectionProfilesByDeviceRepoAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Remove
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/destroyhost")]
        public Task<InvokeResult> RemoveAsync(String id)
        {
            return _instanceManager.DestroyHostAsync(id, OrgEntityHeader, UserEntityHeader);
        }

       
        /// <summary>
        /// Deployment Instance - Regenerate and update access key.
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="key">Currently support either key1 or key2</param>
        /// <returns>Newly Generated Access Key</returns>
        [HttpGet("/api/deployment/instance/{instanceid}/generate/{key}")]
        public Task<InvokeResult<string>> GenrateAccessKey(String instanceid, string key)
        {
            return _instanceManager.RegenerateKeyAsync(instanceid, key, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Instance - Create a credentials object,
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/credentials/factory")]
        public DetailResponse<DeploymentInstanceCredentials> CreateCredentials()
        {
            return DetailResponse<DeploymentInstanceCredentials>.Create();
        }

        /// <summary>
        /// Get Deployment Settings
        /// </summary>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{instanceid}/settings")]
        public Task<InvokeResult<DeploymentSettings>> GetDeploymentSettingsAsync(string instanceid)
        {
            return _instanceManager.GetDeploymentSettingsAsync(instanceid, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/deployment/instance/{instanceId}/account/{username}")]
        public Task<InvokeResult<InstanceAccount>> AddInstanceAccountAsync(string instanceId, string username)
        {
            return _instanceManager.CreateInstanceAccountAsync(instanceId, username, OrgEntityHeader, UserEntityHeader);
        }


        [HttpDelete("/api/deployment/instance/{instanceId}/account/{instanceAccountId}")]
        public Task<InvokeResult> RemoveInstanceAccountAsync(string instanceId, string instanceAccountId)
        {
            return _instanceManager.RemoveInstanceAccountAsync(instanceId, instanceAccountId, OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/deployment/instance/{instanceid}/account/{username}/key/regenerate/{keyname}")]
        public Task<InvokeResult<InstanceAccount>> UpdateInstanceAccountAsync(string instanceid, string username, string keyname)
        {
            return _instanceManager.RegneerateInstanceAccountKeyAsync(instanceid, username, keyname, OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/deployment/instance/{instanceId}/accounts")]
        public async Task<ListResponse<InstanceAccount>> GetInstanceAccountsAsync(string instanceId)
        {
            var accounts = await _instanceManager.GetInstanceAccountsAsync(instanceId, OrgEntityHeader, UserEntityHeader);
            return ListResponse<InstanceAccount>.Create(accounts);
        }
    }

    /// <summary>
    /// Kiosk - Manage Deployment Instances 
    /// </summary>
    [ConfirmedUser]
    [Authorize]
    public class DataStreamDeploymentInstanceController : LagoVistaBaseController
    {
        private readonly IDeploymentInstanceManager _instanceManager;

        public DataStreamDeploymentInstanceController(IDeploymentInstanceManager instanceManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _instanceManager = instanceManager ?? throw new ArgumentNullException(nameof(instanceManager)); ;
        }

        /// <summary>
        /// Web Socket URI - Get a URI to Receive Web Socket Notifications
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="id"></param>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [HttpGet("/api/wsuri/{channel}/{id}/{verbosity}")]
        public Task<InvokeResult<string>> GetMonitorUriAsync(string channel, string id, string verbosity)
        {
            return _instanceManager.GetRemoteMonitoringURIAsync(channel, id, verbosity, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Web Socket URI - Get a URI to Receive Web Socket Notifications
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="orgid"></param>
        /// <param name="repoid"></param>
        /// <param name="id"></param>
        /// <param name="pin"></param>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("/api/wsuri/device/{orgid}/{repoid}/{id}/{verbosity}/{pin}")]
        public Task<InvokeResult<string>> GetDeviceMonitorUriWithPinAsync(string orgid, string repoid, string id, string verbosity, string pin)
        {
            var org = EntityHeader.Create(orgid, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");
            return _instanceManager.GetRemoteMonitoringURIForDeviceWithPINAsync("device", repoid, id, pin, verbosity, org, user);
        }

    }
}