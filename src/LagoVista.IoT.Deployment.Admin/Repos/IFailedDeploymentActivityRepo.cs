// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f7a22e3972bf43bfc7467fc157adfe254d79fcc7d7e6703b8d18b53f96afd741
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IFailedDeploymentActivityRepo
    {
        Task AddFailedDeploymentActivityAsync(DeploymentActivity deploymentActivity);
        Task<IEnumerable<DeploymentActivitySummary>> GetFailedDeploymentActivitiesForResourceIdAsync(string resourceId);
        Task RemoveFailedDeploymentActivityAsync(DeploymentActivity deploymentActivity);
    }
}
