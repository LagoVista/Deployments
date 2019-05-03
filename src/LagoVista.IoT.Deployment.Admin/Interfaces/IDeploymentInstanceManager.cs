using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{

    public interface IDeploymentInstanceManagerRemote
    {
        Task<InvokeResult<DeploymentInstance>> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user);
        Task<DeploymentInstance> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateInstanceStatusAsync(string instanceId, DeploymentInstanceStates newStatus, bool deployed, string version, EntityHeader org, EntityHeader user, string details = "");
    }

    public interface IDeploymentInstanceManager : IDeploymentInstanceManagerRemote
    {

        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteInstanceAsync(string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(NuvIoTEditions edition, string orgId, EntityHeader user);

        Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org);
        Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeployHostAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> StartAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> PauseAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartContainerAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> ReloadSolutionAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateRuntimeAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> StopAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DestroyHostAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult<string>> RegenerateKeyAsync(string instanceId, string keyname, EntityHeader org, EntityHeader user);
        Task<InvokeResult<string>> GetRemoteMonitoringURIAsync(string channel, string id, string verbosity, EntityHeader org, EntityHeader user);
        Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeploymentInstanceStatus>> GetDeploymentInstanceStatusHistoryAsync(string instanceId, EntityHeader org, EntityHeader user, ListRequest listRequest);

        Task<InvokeResult<string>> GetKeyAsync(string keyId, EntityHeader instanceId, EntityHeader org);

        Task<InvokeResult<DeploymentSettings>> GetDeploymentSettingsAsync(string instanceId, EntityHeader org, EntityHeader user);

        string GenerateAccessKey();
    }
}
