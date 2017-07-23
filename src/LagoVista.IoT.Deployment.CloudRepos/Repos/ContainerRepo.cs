using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Linq;
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class ContainerRepo : DocumentDBRepoBase<Container>, IContainerRepo
    {
        private bool _shouldConsolidateCollections;
        public ContainerRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger) : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddContainerAsync(Container container)
        {
            return CreateDocumentAsync(container);
        }

        public Task<Container> GetContainerAsync(string containerId)
        {
            return GetDocumentAsync(containerId);
        }
        
        public async Task<IEnumerable<ContainerSummary>> GetContainersForOrgAsync(string orgId)
        {
            var containers = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true));

            return from item in containers
                   select item.CreateSummary();
        }

        public Task UpdateContainerAsync(Container container)
        {
            return UpsertDocumentAsync(container);
        }
    }
}
