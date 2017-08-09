using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentHostManager
    {
        Task<InvokeResult> AddDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user);

        Task<DeploymentHost> GetNotificationsHostAsync(EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetMCPHostAsync(EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);

        Task<DeploymentHost> LoadFullDeploymentHostAsync(string hostId);

        Task<InvokeResult> DeleteDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeploymentHostSummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user);

        Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org);

        Task<InvokeResult> RegenerateAccessKeys(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeployHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> StartHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateHostAsync(string hostId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> ResetHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> StopHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DestroyHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> PublishAssociatedContainersAsync(string hostId, EntityHeader org, EntityHeader user);


        Task<IEnumerable<DeploymentActivitySummary>> GetHostActivitesAsync(string id, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(String hostId, EntityHeader org, EntityHeader user);

        Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(string hostId, EntityHeader org, EntityHeader user);
    }
}
