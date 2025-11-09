// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7a70330a97a809e6ea40069a059b2bb394c45059a9387324554b061643d2751d
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentActivityQueueManager
    {
        Task Enqueue(Models.DeploymentActivity deploymentActivity);

        Task<IEnumerable<Models.DeploymentActivitySummary>> GetCompletedActivitiesAsync(string resourceId, int take, string before, EntityHeader org, EntityHeader user);
        Task<IEnumerable<Models.DeploymentActivitySummary>> GetFailedActivitiesAsync(string resourceId,  int take, string before, EntityHeader org, EntityHeader user);
        Task<IEnumerable<Models.DeploymentActivitySummary>> GetActiveActivitiesAsync(string resourceId, int take, string before, EntityHeader org, EntityHeader user);
    }
}
