// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 813fdfe70b303692a8bf0864e360909b470990a11c91b528e7bb1774ec06d75d
// IndexVersion: 0
// --- END CODE INDEX META ---
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
using LagoVista.Core.Models.UIMetaData;
using LagoVista.CloudStorage;
using LagoVista.Core.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentInstanceRepo : DocumentDBRepoBase<DeploymentInstance>, IDeploymentInstanceRepo
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly IAdminLogger _adminLogger;

        private bool _shouldConsolidateCollections;
        public DeploymentInstanceRepo(IDeploymentInstanceRepoSettings repoSettings, IAdminLogger logger, IDependencyManager dependencyMgr, ICacheProvider cacheProvider) : 
            base(repoSettings.InstanceDocDbStorage.Uri, repoSettings.InstanceDocDbStorage.AccessKey, repoSettings.InstanceDocDbStorage.ResourceName, logger, dependencyManager: dependencyMgr)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
            _adminLogger = logger;
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider)); // We don't natively cache this object since it can be updated by services that don't have access to the cache, we do provide a readonly version of this object that is cached.
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

        public async Task<DeploymentInstance> GetReadOnlyInstanceAsync(string id)
        {
            var sw = Stopwatch.StartNew();
            var existing = await _cacheProvider.GetAsync($"{nameof(DeploymentInstance)}-{id}");
            if (!String.IsNullOrEmpty(existing))
            {
                _adminLogger.AddCustomEvent(LogLevel.Message, "[DeploymentInstanceRepo__GetReadOnlyInstanceAsync]", $"[DeploymentInstanceRepo__GetReadOnlyInstanceAsync] - Found instance {id} in cache in {sw.Elapsed.TotalMilliseconds}.");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<DeploymentInstance>(existing);
            }

            _adminLogger.AddCustomEvent(LogLevel.Message, "[DeploymentInstanceRepo__GetReadOnlyInstanceAsync]", $"[DeploymentInstanceRepo__GetReadOnlyInstanceAsync] - Cache miss for instance {id} {sw.Elapsed.TotalMilliseconds}.");
            sw.Restart();
            var instance = await GetInstanceAsync(id);
            _adminLogger.AddCustomEvent(LogLevel.Message, "[DeploymentInstanceRepo__GetReadOnlyInstanceAsync]", $"[DeploymentInstanceRepo__GetReadOnlyInstanceAsync] - Laoded {id} from storage in {sw.Elapsed.TotalMilliseconds}.");
            sw.Restart();
            await _cacheProvider.AddAsync($"{nameof(DeploymentInstance)}-{id}", JsonConvert.SerializeObject(instance));
            _adminLogger.AddCustomEvent(LogLevel.Message, "[DeploymentInstanceRepo__GetReadOnlyInstanceAsync]", $"[DeploymentInstanceRepo__GetReadOnlyInstanceAsync] - added {id} to cache in {sw.Elapsed.TotalMilliseconds}.");

            return instance;
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> GetAllActiveInstancesAsync(ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeploymentInstanceSummary, DeploymentInstance>(qry => qry.Status.Value != DeploymentInstanceStates.Offline, ins=>ins.Name, listRequest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> GetAllInstances(ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeploymentInstanceSummary, DeploymentInstance>(qry => true, ins => ins.Name, listRequest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> GetAllFailedInstancesAsync(ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeploymentInstanceSummary, DeploymentInstance>(qry => 
            qry.Status.Value == DeploymentInstanceStates.FailedToStart ||
            qry.Status.Value == DeploymentInstanceStates.FailedToInitialize ||
            qry.Status.Value == DeploymentInstanceStates.FatalError ||
            qry.Status.Value == DeploymentInstanceStates.HostFailedHealthCheck,
            ins=>ins.Name,
            listRequest);
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

            return from item in items.OrderBy(mod => mod.Name)
                   select item.CreateSummary();
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync(string orgId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeploymentInstanceSummary, DeploymentInstance>(qry => qry.OwnerOrganization.Id == orgId && qry.IsArchived == false, ins=>ins.Name, listRequest);
        }

        public async Task<ListResponse<DeploymentInstanceSummary>> GetInstancesForOrgAsync(NuvIoTEditions edition, string orgId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeploymentInstanceSummary, DeploymentInstance>(qry => qry.OwnerOrganization.Id == orgId && (qry.NuvIoTEdition.HasValue && qry.NuvIoTEdition.Value == edition), ins=>ins.Name, listRequest);
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

        public async Task UpdateInstanceAsync(DeploymentInstance instance)
        {
            instance.SharedAccessKey1 = null;
            instance.SharedAccessKey2 = null;

            await _cacheProvider.RemoveAsync($"{nameof(DeploymentInstance)}-{instance.Id}");
            await UpsertDocumentAsync(instance);
        }
    }
}
