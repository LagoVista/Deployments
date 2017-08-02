using LagoVista.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public interface IDeploymentActivityQueueManager
    {
        Task Enqueue(Models.DeploymentActivity deploymentActivity);

        Task<IEnumerable<Models.DeploymentActivity>> GetCompletedActivitiesAsync(string resourceId, int take, string before, EntityHeader org, EntityHeader user);
        Task<IEnumerable<Models.DeploymentActivity>> GetFailedActivitiesAsync(string resourceId,  int take, string before, EntityHeader org, EntityHeader user);
        Task<IEnumerable<Models.DeploymentActivity>> GetActiveActivitiesAsync(string resourceId, int take, string before, EntityHeader org, EntityHeader user);
    }
}
