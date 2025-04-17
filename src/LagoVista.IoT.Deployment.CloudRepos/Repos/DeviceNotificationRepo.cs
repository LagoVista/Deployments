using LagoVista.CloudStorage;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceNotificationRepo : DocumentDBRepoBase<DeviceNotification>, IDeviceNotificationRepo
    {
        private readonly bool _shouldConsolidateCollections;
        private readonly ICacheProvider _cacheProvider;
        private readonly IAdminLogger _logger;

        public DeviceNotificationRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        private string GetCacheKey(string orgId, string customerId, string notificationKey)
        {
            return string.IsNullOrEmpty(customerId) ?  $"notification-{orgId}-{notificationKey}" : $"notification-{orgId}-{customerId}-{notificationKey}";
        }

        public Task<DeviceNotification> GetNotificationAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<DeviceNotification> GetNotificationByKeyAsync(string orgId, string customerId, string key)
        {
            var cacheKey = GetCacheKey(orgId, customerId, key);

            var existing = await _cacheProvider.GetAsync(cacheKey);
            if (existing != null)
            {
                var notification = JsonConvert.DeserializeObject<DeviceNotification>(existing);
                _logger.AddCustomEvent(Core.PlatformSupport.LogLevel.Message, "[DeviceNotificationRepo__GetNotificationByKeyAsync]", $"[DeviceNotificationRepo__GetNotificationByKeyAsync] cache hit {cacheKey} - found notification {notification.Name} ");
                return notification;
            }
            else
            {
                var result = await QueryAsync(dn => dn.OwnerOrganization.Id == orgId && dn.Key == key && (string.IsNullOrEmpty(customerId) || (dn.Customer.Id == customerId || dn.SharedTemplate)));
                if (result == null || !result.Any())
                    throw new RecordNotFoundException(nameof(DeviceNotification), $"Key={key},CustomerId={(String.IsNullOrEmpty(customerId) ? "none" : customerId)}");

                DeviceNotification notification;

                if (!string.IsNullOrEmpty(customerId))
                {
                    notification = result.SingleOrDefault(not => not.Customer?.Id == customerId);
                    if (notification == null)
                        notification = result.SingleOrDefault(not => not.SharedTemplate);
                }
                else
                {
                    notification = result.SingleOrDefault(not => string.IsNullOrEmpty(customerId));
                }

                if(notification == null)
                    throw new RecordNotFoundException(nameof(DeviceNotification), $"Key={key},CustomerId={(String.IsNullOrEmpty(customerId) ? "none" : customerId)}");

                await _cacheProvider.AddAsync(cacheKey, JsonConvert.SerializeObject(notification));

                _logger.AddCustomEvent(Core.PlatformSupport.LogLevel.Message, "[DeviceNotificationRepo__GetNotificationByKeyAsync]", $"[DeviceNotificationRepo__GetNotificationByKeyAsync] cache miss {cacheKey} - added notification {notification.Name} ");

                return notification;
            }        
        }

        public Task<ListResponse<DeviceNotificationSummary>> GetNotificationForOrgAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<DeviceNotificationSummary, DeviceNotification>(ec => ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
        }

        public Task<ListResponse<DeviceNotificationSummary>> GetNotificationTemplatesForOrgAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<DeviceNotificationSummary, DeviceNotification>(ec => ec.OwnerOrganization.Id == orgId && ec.SharedTemplate, ec => ec.Name, listRequest);
        }

        public Task<ListResponse<DeviceNotificationSummary>> GetNotificationForCustomerAsync(string orgId, string customerId, ListRequest listRequest)
        {
            return QuerySummaryAsync<DeviceNotificationSummary, DeviceNotification>(ec => (ec.SharedTemplate || ec.Customer.Id == customerId) && ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
        }

        public async Task UpdateNotificationAsync(DeviceNotification notification)
        {
            await _cacheProvider.RemoveAsync(GetCacheKey(notification.OwnerOrganization.Id, notification.Customer?.Id, notification.Key));
            await UpsertDocumentAsync(notification);
        }
    }
}
