// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: eff5df41a2c5fbe15f0b4360841e033bc91734e985ff5d7f2e31821dbc3548d4
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
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

        Task<DeploymentInstance> GetInstanceAsync(string instanceId);

        Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync(string id, ListRequest listRequest);

        Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync(NuvIoTEditions edition, string id, ListRequest listRequest);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForHostAsync(string id);

        Task<DeploymentInstance> GetReadOnlyInstanceAsync(string id);

        Task<bool> QueryInstanceKeyInUseAsync(string key, string orgId);

        Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(string id);

        Task<ListResponse<DeploymentInstanceSummary>> GetAllInstances(ListRequest listRequest);
        Task<ListResponse<DeploymentInstanceSummary>> GetAllActiveInstancesAsync(ListRequest listRequest);

        Task<ListResponse<DeploymentInstanceSummary>> GetAllFailedInstancesAsync(ListRequest listRequest);
    }
}
