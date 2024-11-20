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
