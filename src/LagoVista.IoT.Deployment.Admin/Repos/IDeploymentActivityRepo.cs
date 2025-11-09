// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9ebdcd6bf6faa8f88d0b4af50a58bc0175d76581a4054edda17f6fd5331526f9
// IndexVersion: 2
// --- END CODE INDEX META ---
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
        Task<IEnumerable<DeploymentActivitySummary>> GetForResourceIdAsync(string resourceId);
        Task<IEnumerable<DeploymentActivity>> GetRunningDeploymentActivitiesAsync();
        Task<IEnumerable<DeploymentActivity>> GetScheduledDeploymentActivitiesAsync();
    }
}
