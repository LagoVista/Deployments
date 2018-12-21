using LagoVista.IoT.Deployment.Admin.Repos;
using System.Collections.Generic;
using System.Linq;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.Core.PlatformSupport;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;
using System;
using LagoVista.Core.Models;
using LagoVista.IoT.Pipeline.Models;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentInstanceRepo : DocumentDBRepoBase<DeploymentInstance>, IDeploymentInstanceRepo
    {

        private bool _shouldConsolidateCollections;
        public DeploymentInstanceRepo(IDeploymentInstanceRepoSettings repoSettings, IAdminLogger logger) : base(repoSettings.InstanceDocDbStorage.Uri, repoSettings.InstanceDocDbStorage.AccessKey, repoSettings.InstanceDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddInstanceAsync(DeploymentInstance instance)
        {
            instance.SharedAccessKey1 = null;
            instance.SharedAccessKey2 = null;

            return CreateDocumentAsync(instance);
        }

        public Task DeleteInstanceAsync(string instanceId)
        {
            return DeleteDocumentAsync(instanceId);
        }

        public async Task<DeploymentInstance> GetInstanceAsync(string instanceId)
        {
            var instance = await GetDocumentAsync(instanceId);
            if (EntityHeader.IsNullOrEmpty(instance.DeploymentConfiguration)) instance.DeploymentConfiguration = EntityHeader<DeploymentConfigurations>.Create(DeploymentConfigurations.SingleInstance);
            if (EntityHeader.IsNullOrEmpty(instance.DeploymentType)) instance.DeploymentType = EntityHeader<DeploymentTypes>.Create(DeploymentTypes.Managed);
            if (EntityHeader.IsNullOrEmpty(instance.QueueType)) instance.QueueType = EntityHeader<QueueTypes>.Create(QueueTypes.InMemory);
            if (EntityHeader.IsNullOrEmpty(instance.LogStorage)) instance.LogStorage = EntityHeader<LogStorage>.Create(LogStorage.Cloud);
            if (EntityHeader.IsNullOrEmpty(instance.PrimaryCacheType)) instance.PrimaryCacheType = EntityHeader<CacheTypes>.Create(CacheTypes.LocalInMemory);

            return instance;
        }

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForHostAsync(string hostId)
        {
            var items = await base.QueryAsync(qry => qry.IsPublic == true || qry.PrimaryHost.Id == hostId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId)
        {
            var items = await base.QueryAsync(qry => qry.IsPublic == true || qry.OwnerOrganization.Id == orgId);

            return from item in items
                   select item.CreateSummary();
        }

        public Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> QueryInstanceKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateInstanceAsync(DeploymentInstance instance)
        {
            instance.SharedAccessKey1 = null;
            instance.SharedAccessKey2 = null;

            return UpsertDocumentAsync(instance);
        }
    }
}
