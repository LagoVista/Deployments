using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IFailedDeploymentActivityRepo
    {
        Task AddFailedDeploymentActivityAsync(DeploymentActivity deploymentActivity);
        Task<IEnumerable<DeploymentActivity>> GetFailedDeploymentActivitiesForResourceIdAsync(string resourceId);
        Task RemoveFailedDeploymentActivityAsync(DeploymentActivity deploymentActivity);
    }
}
