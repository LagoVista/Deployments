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
