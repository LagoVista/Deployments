// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 89b07b993a9b708a0ba24d778bc378d7d1033cc049118afa32f5d24e65f1611f
// IndexVersion: 0
// --- END CODE INDEX META ---
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

        public async Task<ContainerRepository> GetDefaultForRuntimeAsync()
        {
            var containers = await base.QueryAsync(attr => (attr.IsDefaultForRuntime));
            if (containers.Count() > 1)
                throw new System.Exception($"Should only have one default runtime repo, found {containers.Count()}");

            if(containers.Count() == 0)
                throw new System.Exception($"Did not find default runtime repo, please add one");

            return containers.First();
        }

        public Task UpdateContainerRepoAsync(ContainerRepository containerRepository)
        {
            return UpsertDocumentAsync(containerRepository);
        }
    }
}
