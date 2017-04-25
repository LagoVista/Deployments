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
        Task<DeploymentHost> GetDeploymentHostAsync(String hostId);
        Task UpdateDeploymentHostAsync(DeploymentHost host);
        Task DeleteDeploymentHostAsync(String hostId);
        Task<IEnumerable<DeploymentHostSummary>> GetDeploymentsForOrgAsync(string orgId);
        Task<bool> QueryInstanceKeyInUseAsync(string key, string orgId);
    }
}
