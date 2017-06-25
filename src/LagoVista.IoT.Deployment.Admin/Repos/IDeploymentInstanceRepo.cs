using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeploymentInstanceRepo
    {
        Task AddInstanceAsync(DeploymentInstance instance);
        Task UpdateInstanceAsync(DeploymentInstance instance);
        Task DeleteInstanceAsync(string id);

        Task<DeploymentInstance> GetInstanceAsync(string instanceId);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string id);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForHostAsync(string id);
        

        Task<bool> QueryInstanceKeyInUseAsync(string key, string orgId);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(string id);


    }
}
