using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceAdmin.Interfaces.Repos;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceConfigurationRepo : DocumentDBRepoBase<DeviceConfiguration>, IDeviceConfigurationRepo
    {
        private bool _shouldConsolidateCollections;
        public DeviceConfigurationRepo(IDeviceConfigurationSettings repoSettings, IAdminLogger logger) : base(repoSettings.DeviceConfigurationtAdminDocDbStorage.Uri, repoSettings.DeviceConfigurationtAdminDocDbStorage.AccessKey, repoSettings.DeviceConfigurationtAdminDocDbStorage.ResourceName, logger)
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

        public Task AddDeviceConfigurationAsync(DeviceConfiguration deviceConfig)
        {
            return base.CreateDocumentAsync(deviceConfig);
        }

        public Task UpdateDeviceConfigurationAsync(DeviceConfiguration deviceConfig)
        {
            return base.UpsertDocumentAsync(deviceConfig);
        }

        public Task<DeviceConfiguration> GetDeviceConfigurationAsync(String id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<IEnumerable<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgAsync(string orgId)
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

        public Task DeleteDeviceConfigurationAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }
    }
}
