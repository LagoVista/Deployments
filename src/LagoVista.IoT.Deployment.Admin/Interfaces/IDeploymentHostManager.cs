using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentHostManagerRemote
    {
        Task<DeploymentHost> GetDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user, bool throwOnNotFound = true);
        Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(String hostId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeploymentHostStatusAsync(string hostId, HostStatus hostStatus, EntityHeader org, EntityHeader user, string details = "", string cpu = "", string memory = "");
    }

    public interface IDeploymentHostManager : IDeploymentHostManagerRemote
    {
        Task<InvokeResult> AddDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetNotificationsHostAsync(EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetMCPHostAsync(EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeploymentHostSummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user);
        Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org);
        Task<InvokeResult> RegenerateAccessKeys(string id, EntityHeader org, EntityHeader user);
        
        Task<InvokeResult> DeployHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeployContainerAsync(string hostId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DestroyHostAsync(string id, EntityHeader org, EntityHeader user);
        
        Task<IEnumerable<DeploymentActivitySummary>> GetHostActivitesAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(string hostId, EntityHeader org, EntityHeader user);
    }
}
