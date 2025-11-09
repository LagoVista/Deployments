// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c46f2e55b9b90f783b23c428a70437fb87df0a4eb2ea816d29ce55b39f1a0968
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeviceNotificationRepo
    {
        Task AddNotificationAsync(DeviceNotification notificationCode);
        Task DeleteNotificationAsync(string id);
        Task<DeviceNotification> GetNotificationAsync(string id);
        Task<DeviceNotification> GetNotificationByKeyAsync(string orgId, string customerId, string key);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationForOrgAsync(string orgId, ListRequest listRequest);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationTemplatesForOrgAsync(string orgId, ListRequest listRequest);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationForCustomerAsync(string orgId, string customerId, ListRequest listRequest);
        Task UpdateNotificationAsync(DeviceNotification notificationCode);
    }
}
