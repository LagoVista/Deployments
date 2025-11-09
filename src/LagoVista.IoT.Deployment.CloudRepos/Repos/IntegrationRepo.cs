// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b6ffee9d462e72efe244ef22b6ac7a5c6eeefec33e884d5ec7bbc50436661219
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class IntegrationRepo : DocumentDBRepoBase<Integration>,  IIntegrationRepo
    {
        private readonly bool _shouldConsolidateCollections;
        public IntegrationRepo(IDeploymentInstanceRepoSettings repoSettings, IAdminLogger logger) : base(repoSettings.InstanceDocDbStorage.Uri, repoSettings.InstanceDocDbStorage.AccessKey, repoSettings.InstanceDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddIntegrationAsync(Integration integration)
        {
            return CreateDocumentAsync(integration);
        }

        public Task DeleteIntegrationAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<Integration> GetIntegrationAsync(string integrationId)
        {
            return GetDocumentAsync(integrationId);
        }

        public  Task<ListResponse<IntegrationSummary>> GetIntegrationsForOrgsAsync(string orgId, ListRequest listRequest)
        {
           return base.QuerySummaryAsync<IntegrationSummary, Integration>(qry => qry.IsPublic == true || qry.OwnerOrganization.Id == orgId, intg=>intg.Name, listRequest);
         }

        public async Task<bool> QueryKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateIntegrationAsync(Integration integration)
        {
            return UpsertDocumentAsync(integration);
        }
    }
}
