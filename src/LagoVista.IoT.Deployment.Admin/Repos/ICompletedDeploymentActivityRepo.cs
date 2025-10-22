// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 34835b451fca16ca594fe1efbc16f8f65dc0b1898dbd1ff4d94146ea7abf6172
// IndexVersion: 0
// --- END CODE INDEX META ---
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
        Task<IEnumerable<DeploymentActivitySummary>> GetCompletedDeploymentActivitiesForResourceIdAsync(string resourceId);
    }
}
