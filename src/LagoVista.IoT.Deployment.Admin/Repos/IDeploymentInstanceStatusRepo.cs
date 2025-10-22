// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8206e664745f99ee35f8a7655e52b6d3540d0a6262aa83b7b38e99db4664f148
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeploymentInstanceStatusRepo
    {
        Task AddDeploymentInstanceStatusAsync(DeploymentInstanceStatus instanceStatus);
        Task<ListResponse<DeploymentInstanceStatus>> GetStatusHistoryForInstanceAsync(string instanceId, ListRequest listRequest);
    }
}
