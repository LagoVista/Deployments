// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2629cec798a00acbefa3e46c42282b58c33a831454139c198498cfe8a37defd6
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.DocumentDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.CloudStorage.Interfaces;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceConfigurationRepo : DocumentDBRepoBase<DeviceConfiguration>, IDeviceConfigurationRepo
    {
        private bool _shouldConsolidateCollections;
        public DeviceConfigurationRepo(IDeviceConfigurationSettings repoSettings, IDocumentCloudCachedServices services) 
            : base(repoSettings.DeviceConfigurationtAdminDocDbStorage.Uri, repoSettings.DeviceConfigurationtAdminDocDbStorage.AccessKey, 
                  repoSettings.DeviceConfigurationtAdminDocDbStorage.ResourceName, services)
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

        public Task<ListResponse<DeviceConfigurationSummary>> GetDeviceConfigurationsForOrgAsync(string orgId, ListRequest listRequest)
        {
            return base.QuerySummaryAsync<DeviceConfigurationSummary, DeviceConfiguration>(qry => qry.IsPublic == true || qry.OwnerOrganization.Id == orgId, dcf=>dcf.Name, listRequest);
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

        public async Task<string> GetCustomPageForDeviceConfigAsync(string id)
        {
            var doc = await GetDocumentAsync(id);
            return doc.CustomPage;
        }

        public async Task<string> GetQuickLinkCustomPageForDeviceConfigAsync(string id)
        {
            var doc = await GetDocumentAsync(id);
            return doc.CustomPageQuickLink;
        }
    }
}
