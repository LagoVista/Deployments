using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public interface IDeploymentInstanceManager
    {

        Task<DeploymentInstance> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteInstanceAsync(string id, EntityHeader org, EntityHeader user);

        Task<DeploymentInstance> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user);

        Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org);

        Task<InvokeResult> DeployAsync(String id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> StartAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> PauseAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartHostAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> ReloadSolutionAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> StopAsync(String id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RemoveAsync(String id, EntityHeader org, EntityHeader user);

        Task<InvokeResult<string>> GetRemoteMonitoringURIAsync(string channel, string id, string verbosity, EntityHeader org, EntityHeader user);
        Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user);
    }
}
