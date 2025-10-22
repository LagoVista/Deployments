// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 19cb1530d014eab0bb64f36a771672685c9c3e8e9724e80bb4e16831d1743e8c
// IndexVersion: 0
// --- END CODE INDEX META ---
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
