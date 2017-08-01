using LagoVista.IoT.Deployment.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeploymentActivityRepo
    {
        Task AddDeploymentActivityAsync(DeploymentActivity deploymentActivity);
        Task UpdateDeploymentActivityAsync(DeploymentActivity deploymentActivity);
        Task RemoveDeploymentActivityAsync(DeploymentActivity deploymentActivity);
        Task<DeploymentActivity> GetDeploymentActivityAsync(string deploymentActivityId);
        Task<IEnumerable<DeploymentActivity>> GetRetryDeploymentActivitiesAsync();
        Task<IEnumerable<DeploymentActivity>> GetForResourceIdAsync(string resourceId);
        Task<IEnumerable<DeploymentActivity>> GetActiveDeploymentActivitiesAsync();
    }
}
