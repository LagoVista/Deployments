using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class RemoteDeploymentRepo : DocumentDBRepoBase<RemoteDeployment>, IRemoteDeploymentRepo
    {
        private bool _shouldConsolidateCollections;
        public RemoteDeploymentRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;


        public Task AddRemoteDeploymentAsync(RemoteDeployment deployment)
        {
            return CreateDocumentAsync(deployment);
        }

        public Task DeleteRemoteDeploymentAsync(string id)
        {
            return base.DeleteDocumentAsync(id);
        }

        public Task<RemoteDeployment> GetRemoteDeploymentAsync(string id, bool populateChildren = false)
        {
            return GetDocumentAsync(id);
        }

        public async Task<ListResponse<RemoteDeploymentSummary>> GetRemoteDeploymentsForOrgsAsync(string orgId)
        {
            var containers = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true));

            return ListResponse<RemoteDeploymentSummary>.Create(from item in containers
                                                                select item.CreateSummary());
        }

        public async Task<bool> QueryKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateRemoteeDeploymentAsync(RemoteDeployment deployment)
        {
            return UpsertDocumentAsync(deployment);
        }
    }
}
