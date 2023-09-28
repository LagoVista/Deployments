using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeploymentHostRepo
    {
        Task AddDeploymentHostAsync(DeploymentHost host);
        Task<DeploymentHost> GetDeploymentHostForDedicatedInstanceAsync(String instanceId);
        Task<DeploymentHost> GetDeploymentHostAsync(String hostId, bool throwOnNotFound = true);
        Task<DeploymentHost> GetNotificationsHostAsync();
        Task<DeploymentHost> GetMCPHostAsync();

        Task UpdateDeploymentHostAsync(DeploymentHost host);
        Task<IEnumerable<DeploymentHostSummary>> GetDeploymentsForOrgAsync(string orgId);
        Task<bool> QueryHostKeyInUse(string key, string orgId);
        Task<DeploymentHost> FindSharedHostAsync(HostTypes hostType);

        Task<ListResponse<DeploymentHostSummary>> GetAllHostsAsync(ListRequest listRequest);
        Task<ListResponse<DeploymentHostSummary>> GetAllActiveHostsAsync(ListRequest listRequest);

        Task<ListResponse<DeploymentHostSummary>> GetAllFailedHostsAsync(ListRequest listRequest);
    }
}

