using LagoVista.CloudStorage.DocumentDB;
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
        public ClientAppRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger)
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

        public async Task<ListResponse<ClientAppSummary>> GetClientAppsForOrgAsync(string orgId, ListRequest requst)
        {
            var response = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true), requst);
            //TODO: This is a broken pattern to be fixed another day...sorry.
            var finalResponse = ListResponse<ClientAppSummary>.Create( response.Model.OrderBy(mod => mod.Name).Select(mod => mod.CreateSummary()));
            finalResponse.NextPartitionKey = response.NextPartitionKey;
            finalResponse.NextRowKey = response.NextRowKey;
            finalResponse.PageCount = response.PageCount;
            finalResponse.PageCount = response.PageIndex;
            finalResponse.PageSize = response.PageSize;
            finalResponse.ResultId = response.ResultId;
            return finalResponse;
            
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
