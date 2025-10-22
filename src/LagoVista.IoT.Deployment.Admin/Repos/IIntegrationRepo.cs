// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 896076103256a8cd56c5ffbf009253f16207386525a8e485671f87931feba39b
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IIntegrationRepo
    {
        Task AddIntegrationAsync(Integration integration);
        Task<Integration> GetIntegrationAsync(string integrationId);
        Task UpdateIntegrationAsync(Integration integration);
        Task<ListResponse<IntegrationSummary>> GetIntegrationsForOrgsAsync(string orgId, ListRequest listRequest);
        Task<bool> QueryKeyInUseAsync(string key, string id);
        Task DeleteIntegrationAsync(string id);
    }
}
