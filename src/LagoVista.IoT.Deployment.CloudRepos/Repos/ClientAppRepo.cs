using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos
{
    public class ClientAppRepo : DocumentDBRepoBase<ClientApp>, IClientAppRepo
    {
        private bool _shouldConsolidateCollections;
        public ClientAppRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, IDependencyManager dependency, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider, dependencyManager:dependency)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddClientAppAsync(ClientApp app)
        {
            return CreateDocumentAsync(app);
        }

        public Task DeleteClientAppAsync(string id)
        {
            return base.DeleteDocumentAsync(id);
        }

        public Task<ClientApp> GetClientAppAsync(string id)
        {
            return GetDocumentAsync(id, false);
        }

        public async Task<ClientApp> GetKioskClientAppAsync(string orgId, string kioskId)
		{
            var clientApps = await base.QueryAsync(attr => attr.OwnerOrganization.Id == orgId && attr.Kiosk != null && attr.Kiosk.Id == kioskId);
            if (clientApps.Any())
            {
                return clientApps.First();
            }
            return default;
        }

        public Task<ListResponse<ClientAppSummary>> GetClientAppsForOrgAsync(string orgId, ListRequest requst)
        {
            return base.QuerySummaryAsync<ClientAppSummary, ClientApp>(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true), app=>app.Name, requst);
        }

        public async Task<bool> QueryKeyInUseAsync(string key, string orgId)
        {
            return (await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key)).Any();
        }

        public Task UpdateClientAppAsync(ClientApp clientApp)
        {
            return UpsertDocumentAsync(clientApp);
        }
    }
}
