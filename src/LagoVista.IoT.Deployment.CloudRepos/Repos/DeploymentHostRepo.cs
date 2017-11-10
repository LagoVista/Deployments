using LagoVista.IoT.Deployment.Admin.Repos;
using System.Collections.Generic;
using System.Linq;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Logging.Exceptions;
using LagoVista.IoT.Deployment.Admin.Resources;

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

        public Task DeleteDeploymentHostAsync(string hostId)
        {
            return DeleteDocumentAsync(hostId);
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
            var items = await base.QueryAsync(qry => qry.IsPublic == true || qry.OwnerOrganization.Id == orgId);

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
