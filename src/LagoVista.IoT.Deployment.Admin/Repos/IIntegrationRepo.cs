using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IIntegrationRepo
    {
        Task AddIntegrationAsync(Integration integration);
        Task<Integration> GetIntegrationAsync(string integrationId);
        Task UpdateIntegrationAsync(Integration integration);
        Task<IEnumerable<IntegrationSummary>> GetIntegrationsForOrgsAsync(string orgId);
        Task<bool> QueryKeyInUseAsync(string key, string id);

        Task DeleteIntegrationAsync(string id);
    }
}
