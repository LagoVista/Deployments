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
    }
}
