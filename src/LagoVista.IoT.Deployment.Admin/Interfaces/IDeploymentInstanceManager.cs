using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{

    public interface IDeploymentInstanceManagerRemote
    {
        Task<InvokeResult<DeploymentInstance>> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user);        
        Task<DeploymentInstance> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateInstanceStatusAsync(string instanceId, DeploymentInstanceStates newStatus, bool deployed, EntityHeader org, EntityHeader user, string details = "");
    }

    public interface IDeploymentInstanceManager : IDeploymentInstanceManagerRemote
    {

        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteInstanceAsync(string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user);

        Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org);
        Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeployHostAsync(String id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> StartAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> PauseAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartHostAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartContainerAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> ReloadSolutionAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateRuntimeAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> StopAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DestroyHostAsync(String id, EntityHeader org, EntityHeader user);

        Task<InvokeResult<string>> GetRemoteMonitoringURIAsync(string channel, string id, string verbosity, EntityHeader org, EntityHeader user);
        Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user);


    }
}
