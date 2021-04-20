using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IClientAppRepo
    {
        Task AddClientAppAsync(ClientApp app);                    
        Task<ClientApp> GetClientAppAsync(string id);
        Task<ClientApp> GetKioskClientAppAsync(string orgId, string kioskId);
        Task<ListResponse<ClientAppSummary>> GetClientAppsForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateClientAppAsync(ClientApp deployment);
        Task DeleteClientAppAsync(string id);
        Task<bool> QueryKeyInUseAsync(string key, string org);
    }
}
