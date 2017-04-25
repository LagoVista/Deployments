using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.PlatformSupport;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceMessageDefinitionRepo : DocumentDBRepoBase<DeviceMessageDefinition>, IDeviceMessageDefinitionRepo
    {
        private bool _shouldConsolidateCollections;
        public DeviceMessageDefinitionRepo(IDeviceConfigurationSettings repoSettings, ILogger logger) : base(repoSettings.DeviceConfigurationtAdminDocDbStorage.Uri, repoSettings.DeviceConfigurationtAdminDocDbStorage.AccessKey, repoSettings.DeviceConfigurationtAdminDocDbStorage.ResourceName, logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        protected override bool ShouldConsolidateCollections
        {
            get
            {
                return _shouldConsolidateCollections;
            }
        }

        public Task AddDeviceMessageDefinitionAsync(DeviceMessageDefinition deviceMessageDefinition)
        {
            return base.CreateDocumentAsync(deviceMessageDefinition);
        }

        public Task DeleteDeviceMessageDefinitionAsync(string id)
        {
            return base.DeleteDocumentAsync(id);
        }

        public Task<DeviceMessageDefinition> GetDeviceMessageDefinitionAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<IEnumerable<DeviceMessageDefinitionSummary>> GetDeviceMessageDefinitionsForOrgAsync(string orgId)
        {
            var items = await base.QueryAsync(qry => qry.IsPublic == true || qry.OwnerOrganization.Id == orgId);

            return from item in items
                   select item.CreateSummary();
        }

        public async Task<bool> QueryKeyInUseAsync(string key, string orgId)
        {
            var items = await base.QueryAsync(attr => (attr.OwnerOrganization.Id == orgId || attr.IsPublic == true) && attr.Key == key);
            return items.Any();
        }

        public Task UpdateDeviceMessageDefinitionAsync(DeviceMessageDefinition deviceMessageDefinition)
        {
            return base.UpsertDocumentAsync(deviceMessageDefinition);
        }
    }
}
