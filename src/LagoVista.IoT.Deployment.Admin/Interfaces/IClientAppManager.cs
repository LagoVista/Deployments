// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bc0baa65bb32ab675b9ffeb101fb59a7fac92720f5fb5652acad93683396c540
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;


namespace LagoVista.IoT.Deployment.Admin
{
    public interface IClientAppManager
    {
        Task<InvokeResult<ClientApp>> AuthorizeAppAsync(string clientAppId, string apiKey);

        Task<InvokeResult> AddClientAppAsync(ClientApp clientApp, EntityHeader org, EntityHeader user);
        Task<ClientApp> GetClientAppAsync(string id, EntityHeader org, EntityHeader user);
        Task<KioskClientAppSummary> GetKioskClientAppAsync(string kioskId, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<ClientAppSummary>> GetClientAppsForOrgAsync(string id, EntityHeader user, ListRequest request);
        Task<InvokeResult<ClientAppSecrets>> GetClientAppSecretsAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateClientAppAsync(ClientApp clientApp, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteClientAppAsync(string id, EntityHeader org, EntityHeader user);
        Task<bool> QueryKeyInUse(string key, EntityHeader org);

        string GenerateAPIKey();
    }

}
