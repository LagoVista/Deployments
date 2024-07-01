using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class SystemTestRepo : DocumentDBRepoBase<SystemTest>, ISystemTestRepo
    {
        private bool _shouldConsolidateCollections;

        public SystemTestRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }
        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddSystemTestAsync(SystemTest systemTest)
        {
            return CreateDocumentAsync(systemTest);
        }

        public Task DeleteSystemTestAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<SystemTest> GetSystemTestAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<SystemTest> GetSystemTestByKeyAsync(string key, string orgId)
        {
            var result = await QueryAsync(erc => erc.Key == key && erc.OwnerOrganization.Id == orgId);
            return result.FirstOrDefault();
        }

        public Task<ListResponse<SystemTestSummary>> GetSystemTestsForOrgAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<SystemTestSummary, SystemTest>(ec => ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
        }

        public Task UpdateSystemTestAsync(SystemTest systemTest)
        {
            return UpsertDocumentAsync(systemTest);
        }
    }
}
