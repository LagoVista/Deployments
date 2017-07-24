using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeploymentActivityRepo
    {
        Task AddDeploymentActivityAsync(DeploymentActivity deploymentActivity);
        Task UpdateDeploymentActivityAsync(DeploymentActivity deploymentActivity);
        Task<List<DeploymentActivity>> GetForResourceIdAsync(string resourceId);
    }
}
