// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 579ae48e35ba000529517092acdb00410f1519a4edd121ed8be76a21dc858da2
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Repos;
using System.Collections.Generic;
using System.Linq;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Exceptions;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.Core.Exceptions;
using LagoVista.CloudStorage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Interfaces;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentHostRepo : DocumentDBRepoBase<DeploymentHost>, IDeploymentHostRepo
    {
        private readonly bool _shouldConsolidateCollections;
        public DeploymentHostRepo(IDeploymentInstanceRepoSettings repoSettings, IAdminLogger logger, IDependencyManager dependencyMgr) : 
            base(repoSettings.InstanceDocDbStorage.Uri, repoSettings.InstanceDocDbStorage.AccessKey, repoSettings.InstanceDocDbStorage.ResourceName, logger, dependencyManager: dependencyMgr)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public async Task AddDeploymentHostAsync(DeploymentHost host)
        {
            if(host.HostType.Value == HostTypes.MCP)
            {
                var mcps = await base.QueryAsync(qry => qry.HostType.Value == HostTypes.MCP);
                if (mcps.Any())
                {
                    throw new InvalidConfigurationException(DeploymentErrorCodes.MCPExists);
                }
            }

            if (host.HostType.Value == HostTypes.Notification)
            {
                var notificationsServers = await base.QueryAsync(qry => qry.HostType.Value == HostTypes.Notification);
                if(notificationsServers.Any())
                {
                    throw new InvalidConfigurationException(DeploymentErrorCodes.NotificationsServerExists);
                }
            }

            await CreateDocumentAsync(host);
        }

        public async Task<DeploymentHost> FindSharedHostAsync(HostTypes hostType)
        {
            var hosts = await QueryAsync(qry => qry.HostType.Value == hostType && !qry.IsArchived);
            if (hosts.Count() == 0)
            {
                throw new RecordNotFoundException("DeploymentHost", "Shared=true");
            }

            var assignableHost = hosts.OrderBy(hsts =>  hsts.DeployedInstances.Count).FirstOrDefault();

            return assignableHost;
        }

        public async Task<ListResponse<DeploymentHostSummary>> GetAllActiveHostsAsync(ListRequest listRequest)
        {
            var items = await base.QueryAsync(qry => qry.Status.Value != HostStatus.Offline, srt => srt.Name, listRequest);
            var summaryItems = items.Model.OrderBy(mod => mod.Name).Select(di => di.CreateSummary());
            return ListResponse<DeploymentHostSummary>.Create(listRequest, summaryItems);
        }

        public async Task<ListResponse<DeploymentHostSummary>> GetAllFailedHostsAsync(ListRequest listRequest)
        {
            var items = await base.QueryAsync(qry => qry.Status.Value != HostStatus.Offline && qry.Status.Value != HostStatus.Running, srt => srt.Name, listRequest);
            var summaryItems = items.Model.OrderBy(mod => mod.Name).Select(di => di.CreateSummary());
            return ListResponse<DeploymentHostSummary>.Create(listRequest, summaryItems);
        }

        public async Task<ListResponse<DeploymentHostSummary>> GetAllHostsAsync(ListRequest listRequest)
        {
            var items = await base.QueryAsync(qry => true, srt=>srt.Name, listRequest);
            var summaryItems = items.Model.OrderBy(mod => mod.Name).Select(di => di.CreateSummary());
            return ListResponse<DeploymentHostSummary>.Create(listRequest, summaryItems);
        }

        public Task<DeploymentHost> GetDeploymentHostAsync(string hostId, bool throwOnNotFound = true)
        {
            return GetDocumentAsync(hostId, throwOnNotFound);
        }

        public async Task<DeploymentHost> GetDeploymentHostForDedicatedInstanceAsync(string instanceId)
        {
            return (await QueryAsync(host => host.DedicatedInstance.Id == instanceId)).FirstOrDefault();
        }

        public async Task<IEnumerable<DeploymentHostSummary>> GetDeploymentsForOrgAsync(string orgId)
        {
            var items = await base.QueryAsync(qry => qry.IsArchived == false && (qry.IsPublic == true || qry.OwnerOrganization.Id == orgId));

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<DeploymentHost> GetMCPHostAsync()
        {
            var items = await base.QueryAsync(qry => qry.HostType.Value == HostTypes.MCP);

            if (items.Count() == 0)
            {
                throw new InvalidConfigurationException(DeploymentErrorCodes.NoMCPExits);
            }

            if(items.Count() > 1)
            {
                throw new InvalidConfigurationException(DeploymentErrorCodes.MultipleMCPServersFound);
            }

            return items.First();
        }

        public async Task<DeploymentHost> GetNotificationsHostAsync()
        {
            var items = await base.QueryAsync(qry => qry.HostType.Value == HostTypes.Notification);

            if (items.Count() == 0)
            {
                throw new InvalidConfigurationException(DeploymentErrorCodes.NoNotificationsServerExits);
            }

            if (items.Count() > 1)
            {
                throw new InvalidConfigurationException(DeploymentErrorCodes.MultipleNotificationServersFound);
            }

            return items.First();
        }


        public async Task<bool> QueryHostKeyInUse(string key, string orgId)
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
