using LagoVista.IoT.Deployment.Admin.Repos;
using System.Collections.Generic;
using System.Linq;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.CloudStorage.DocumentDB;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class ContainerRepositoryRepo : DocumentDBRepoBase<ContainerRepository>, IContainerRepositoryRepo
    {
        private bool _shouldConsolidateCollections;
        public ContainerRepositoryRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger) 
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;


        public Task AddContainerRepoAsync(ContainerRepository container)
        {
            return CreateDocumentAsync(container);
        }

        public Task<ContainerRepository> GetContainerRepoAsync(string containerRepoId)
        {
            return GetDocumentAsync(containerRepoId);
        }

        public async Task<IEnumerable<ContainerRepositorySummary>> GetContainerReposForOrgAsync(string orgId)
        {
            var containers = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true));

            return from item in containers
                   select item.CreateSummary();
        }

        public Task UpdateContainerRepoAsync(ContainerRepository containerRepository)
        {
            return UpsertDocumentAsync(containerRepository);
        }
    }
}
