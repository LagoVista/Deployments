// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2574229860b265af3b5c7454dcd37e825c46f3425bf8c99706159ad46f765a68
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.Core.PlatformSupport;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class SolutionRepo : DocumentDBRepoBase<Solution>, ISolutionRepo
    {

        private bool _shouldConsolidateCollections;
        public SolutionRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, IDependencyManager dependencyMgr) : 
            base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, dependencyManager: dependencyMgr)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;


        public Task AddSolutionAsync(Solution deployment)
        {
            return base.CreateDocumentAsync(deployment);
        }

        public Task DeleteSolutionAsync(string  id)
        {
            return base.DeleteDocumentAsync(id);
        }

        public Task<Solution> GetSolutionAsync(string id, bool populateChildren = false)
        {
            return GetDocumentAsync(id);
        }

        public async Task<ListResponse<SolutionSummary>> GetSolutionsForOrgsAsync(string orgId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<SolutionSummary, Solution>(qry => qry.IsPublic == true || qry.OwnerOrganization.Id == orgId, sol=>sol.Name, listRequest);
        }

        public async Task<bool> QueryKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateSolutionAsync(Solution deployment)
        {
            return base.UpsertDocumentAsync(deployment);
        }
    }
}