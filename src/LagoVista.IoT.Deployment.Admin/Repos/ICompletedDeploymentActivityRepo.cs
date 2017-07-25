using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface ICompletedDeploymentActivityRepo
    {
        Task AddCompletedDeploymentActivitiesAsync(DeploymentActivity deploymentActivity);
        Task<IEnumerable<DeploymentActivity>> GetCompletedDeploymentActivitiesForResourceIdAsync(string resourceId);
    }
}
