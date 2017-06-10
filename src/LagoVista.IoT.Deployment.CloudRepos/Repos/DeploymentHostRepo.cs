using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentHostRepo : DocumentDBRepoBase<DeploymentHost>, IDeploymentHostRepo
    {
        private bool _shouldConsolidateCollections;
        public DeploymentHostRepo(IDeploymentInstanceRepoSettings repoSettings, IAdminLogger logger) : base(repoSettings.InstanceDocDbStorage.Uri, repoSettings.InstanceDocDbStorage.AccessKey, repoSettings.InstanceDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;


        public Task AddDeploymentHostAsync(DeploymentHost host)
        {
            return CreateDocumentAsync(host);
        }

        public Task DeleteDeploymentHostAsync(string hostId)
        {
            return DeleteDocumentAsync(hostId);
        }

        public Task<DeploymentHost> GetDeploymentHostAsync(string hostId)
        {
            return GetDocumentAsync(hostId);
        }

        public async Task<IEnumerable<DeploymentHostSummary>> GetDeploymentsForOrgAsync(string orgId)
        {
            var items = await base.QueryAsync(qry => qry.IsPublic == true || qry.OwnerOrganization.Id == orgId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<bool> QueryInstanceKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateDeploymentHostAsync(DeploymentHost host)
        {
            return UpsertDocumentAsync(host);
        }
    }
}
