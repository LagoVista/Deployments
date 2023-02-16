using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceAdmin.Models;
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
    [OrgAdmin]
    [Authorize]
    public class DeploymentInstanceController : LagoVistaBaseController
    {
        private readonly IDeploymentInstanceManager _instanceManager;
        private readonly ITimeZoneServices _timeZoneServices;

        public DeploymentInstanceController(IDeploymentInstanceManager instanceManager, ITimeZoneServices timeZoneServices, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _instanceManager = instanceManager ?? throw new ArgumentNullException(nameof(instanceManager)); ;
            _timeZoneServices = timeZoneServices ?? throw new ArgumentNullException(nameof(timeZoneServices));
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
        public Task<ListResponse<DeploymentInstanceSummary>> GetAllInstancesAsync()
        {
            return _instanceManager.SysAdminGetAllInstancesAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/instances/{orgid}")]
        public Task<ListResponse<DeploymentInstanceSummary>> GetAllInstancesAsync(string orgid)
        {
            return _instanceManager.SysAdminGetInstancesAsync(orgid, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/instances/active")]
        public Task<ListResponse<DeploymentInstanceSummary>> GetAllActiveInstancesAsync()
        {
            return _instanceManager.SysAdminGetActiveInstancesAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        [SystemAdmin]
        [HttpGet("/sys/api/deployment/instances/failed")]
        public Task<ListResponse<DeploymentInstanceSummary>> GetFailedInstancesAsync()
        {
            return _instanceManager.SysAdminFailedInstancesAsync(OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
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

            var response = DetailResponse<DeploymentInstance>.Create(deviceInstance);
            response.View["timeZone"].Options = _timeZoneServices.GetTimeZones().Select(tz => new EnumDescription() { Key = tz.Id, Label = tz.DisplayName, Name = tz.DisplayName }).ToList();

            return response;
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
        public async Task<InvokeResult<ListenerConfiguration>>  GetDefaultListenerConfigForRepoAsync(String repoid)
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
        public Task<ListResponse<WatchdogConnectedDevice>> GetTimedoutDevicesAsync(string id)
        {
            return _instanceManager.GetWatchdogConnectedDevicesAsync(id, OrgEntityHeader, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Get connected devices that have timed out.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/connected/timedout")]
        public Task<ListResponse<WatchdogConnectedDevice>> GetWatchdogConnectedDevicesAsync(string id)
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
        public DetailResponse<DeploymentInstance> CreateInstance()
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
        /// Deployment Instance - Stop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{id}/stop")]
        public Task<InvokeResult> StopAsync(String id)
        {
            return _instanceManager.StopAsync(id, OrgEntityHeader, UserEntityHeader);
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
            var profile = new WiFiConnectionProfile()
            {
                Id = Guid.NewGuid().ToId(),
            };

            return DetailResponse<WiFiConnectionProfile>.Create(profile);
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
        /// Get Deployment Settings
        /// </summary>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/instance/{instanceid}/settings")]
        public Task<InvokeResult<DeploymentSettings>> GetDeploymentSettingsAsync(string instanceid)
        {
            return _instanceManager.GetDeploymentSettingsAsync(instanceid, OrgEntityHeader, UserEntityHeader);
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
    }
}