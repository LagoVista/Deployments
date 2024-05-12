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
        Task<DeviceNotification> GetNotificationByKeyAsync(string orgId, string key);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateNotificationAsync(DeviceNotification notificationCode);
    }
}
