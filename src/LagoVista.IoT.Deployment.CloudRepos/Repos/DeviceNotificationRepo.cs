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
        private bool _shouldConsolidateCollections;
        private ICacheProvider _cacheProvider;

        public DeviceNotificationRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
            _cacheProvider = cacheProvider;
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
            var existing = await _cacheProvider.GetAsync(GetCacheKey(orgId, customerId, key));
            if (existing != null)
            {
                return JsonConvert.DeserializeObject<DeviceNotification>(existing);
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

                var notiifcation = result.First();
                await _cacheProvider.AddAsync(GetCacheKey(orgId, customerId, key), JsonConvert.SerializeObject(notiifcation));
                return notiifcation;
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
