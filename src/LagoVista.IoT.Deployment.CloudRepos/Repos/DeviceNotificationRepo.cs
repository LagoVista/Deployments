using LagoVista.CloudStorage;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceNotificationRepo : DocumentDBRepoBase<DeviceNotification>, IDeviceNotificationRepo
    {
        private bool _shouldConsolidateCollections;

        public DeviceNotificationRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }
        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddNotificationAsync(DeviceNotification errorCode)
        {
            return CreateDocumentAsync(errorCode);
        }

        public Task DeleteNotificationAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<DeviceNotification> GetNotificationAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<DeviceNotification> GetNotificationByKeyAsync(string orgId, string key)
        {
            var result = await QueryAsync(dn=>dn.OwnerOrganization.Id == orgId && dn.Key == key);
            if (result == null || !result.Any())
                throw new RecordNotFoundException(nameof(DeviceNotification), $"Key={key}");

            return result.First();
        
        }

        public Task<ListResponse<DeviceNotificationSummary>> GetNotificationForOrgAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<DeviceNotificationSummary, DeviceNotification>(ec => ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
        }

        public Task UpdateNotificationAsync(DeviceNotification errorCode)
        {
            return UpsertDocumentAsync(errorCode);
        }
    }
}
