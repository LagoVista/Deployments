using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public interface IDeploymentHostManager
    {
        Task<InvokeResult> AddDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetDeploymentHostAsync(String hostId, EntityHeader org, EntityHeader user);

        Task<DeploymentHost> GetNotificationsHostAsync(EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetMCPHostAsync(EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);

        Task<DeploymentHost> LoadFullDeploymentHostAsync(String hostId);

        Task<InvokeResult> DeleteDeploymentHostAsync(String hostId, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeploymentHostSummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user);

        Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org);

        Task<InvokeResult> RegenerateAccessKeys(String id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(String hostId, EntityHeader org, EntityHeader user);
    }
}
