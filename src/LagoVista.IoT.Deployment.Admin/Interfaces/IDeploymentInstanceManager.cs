// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 641d347a0d6e2e7888b9bf8e430dfba86d3f59b54028105f15e8538cbe8bdf39
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.IoT.Pipeline.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{

    public interface IDeploymentInstanceManagerRemote
    {
        Task<InvokeResult<DeploymentInstance>> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeploymentInstance>> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateInstanceStatusAsync(string instanceId, DeploymentInstanceStates newStatus, bool deployed, string version, EntityHeader org, EntityHeader user, string details = "");
    }

    public interface IDeploymentInstanceManager : IDeploymentInstanceManagerRemote
    {

        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteInstanceAsync(string id, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);

        Task<ListResponse<DeploymentInstanceSummary>> SysAdminGetAllInstancesAsync(EntityHeader org, EntityHeader user, ListRequest listRqeuest);
        Task<ListResponse<DeploymentInstanceSummary>> SysAdminGetInstancesAsync(string orgId, EntityHeader org, EntityHeader user, ListRequest listRqeuest);
        Task<ListResponse<DeploymentInstanceSummary>> SysAdminGetActiveInstancesAsync(EntityHeader org, EntityHeader user, ListRequest listRqeuest);
        Task<ListResponse<DeploymentInstanceSummary>> SysAdminFailedInstancesAsync(EntityHeader org, EntityHeader user, ListRequest listRqeuest);

        Task<ListResponse<DeploymentInstanceSummary>> GetInstanceForOrgAsync(NuvIoTEditions edition, string orgId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpgradeInstanceAsync(string instanceId, string imageId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddToShardHostAsync(string hostId, string instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> RemoveSharedInstanceAsync(string hostId, string instanceId, EntityHeader org, EntityHeader user);

        Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org);
        Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeployHostAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> StartAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> ResetAppAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> PauseAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartContainerAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> SetContainerRepoAsync(string instanceId, string containerRepoId, string tagId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> ReloadSolutionAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateRuntimeAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> StopAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DestroyHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ListenerConfiguration>> GetDefaultListenerConfigurationAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<ListenerConfiguration>> GetDefaultListenerConfigurationForRepoAsync(string repoId, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceTypeSummary>> GetDeviceTypesForInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceStatus>> GetStatusHistoryForDeviceAsync(string instanceId, string deviceUniqueId, EntityHeader org, EntityHeader user, ListRequest listRequest);

        Task<InvokeResult<string>> RegenerateKeyAsync(string instanceId, string keyname, EntityHeader org, EntityHeader user);
        Task<InvokeResult<string>> GetRemoteMonitoringURIAsync(string channel, string id, string verbosity, EntityHeader org, EntityHeader user);
        Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeploymentInstanceStatus>> GetDeploymentInstanceStatusHistoryAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest);

        Task<InvokeResult<string>> GetKeyAsync(string keyId, EntityHeader instanceId, EntityHeader org);

        Task<ListResponse<DeviceStatus>> GetWatchdogConnectedDevicesAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest);

        Task<InvokeResult<DeploymentSettings>> GetDeploymentSettingsAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceStatus>> GetTimedoutDevicesAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest);

        Task<ListResponse<WatchdogMessageStatus>> GetWatchdogMessageStatusAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest);

        Task<ListResponse<WatchdogMessageStatus>> GetTimedOutWatchdogMessageStatusAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult<InstanceAccount>> RegneerateInstanceAccountKeyAsync(string instanceId, string userName, string keyName, EntityHeader org, EntityHeader user);

        Task<InvokeResult<InstanceAccount>> CreateInstanceAccountAsync(string instanceId, string userName, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RemoveInstanceAccountAsync(string instanceId, string id, EntityHeader org, EntityHeader user);

        Task<List<InstanceAccount>> GetInstanceAccountsAsync(string instanceId, EntityHeader org, EntityHeader user);

        Task<InvokeResult<WiFiConnectionProfile>> GetWiFiConnectionProfileAsync(string instanceId, string profileId, EntityHeader org, EntityHeader user);
        Task<List<WiFiConnectionProfile>> GetWiFiConnectionProfilesByDeviceRepoAsync(string repoId, EntityHeader org, EntityHeader user);

        string GenerateAccessKey();
        Task<InvokeResult<InstanceService>> AllocateInstanceServiceAsync(string instanceId, HostTypes hostType, bool replaceExisting, EntityHeader orgEntityHeader, EntityHeader userEntityHeader);
        Task<InvokeResult> RemoveInstanceServiceAsync(string instanceId, string instanceServiceId, EntityHeader orgEntityHeader, EntityHeader userEntityHeader);

        Task<InvokeResult> SetSilenceAlarmAsync(string instanceId, string id, bool silenced, EntityHeader org, EntityHeader user);

        Task<InvokeResult<bool>> DeploymentInDebugMode(string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<bool>> DeploymentInTestMode(string instanceId, EntityHeader org, EntityHeader user);

    }
}
