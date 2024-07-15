using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class SystemTestExecutionRepo : DocumentDBRepoBase<SystemTestExecution>, ISystemTestExecutionRepo
    {
        private bool _shouldConsolidateCollections;

        public SystemTestExecutionRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }
        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;


        public Task AddSystemTestExecutionAsync(SystemTestExecution testExecution)
        {
            return CreateDocumentAsync(testExecution);
        }

        public Task DeleteTestExecutionAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<SystemTestExecution> GetSystemTestExecutionAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public Task<ListResponse<SystemTestExecutionSummary>> GetSystemTestExecutionsAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<SystemTestExecutionSummary, SystemTestExecution>(ec => ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
        }

        public Task UpdateSystemTestExecutionAsync(SystemTestExecution testExecution)
        {
            return UpsertDocumentAsync(testExecution);
        }
    }
}
