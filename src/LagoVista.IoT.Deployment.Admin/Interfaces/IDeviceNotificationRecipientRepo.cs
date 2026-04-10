using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IDeviceNotificationRecipientRepo
    {
        Task AddNotificationRecipientAsync(DeviceNotificationRecipient recipient);
        Task<ListResponse<DeviceNotificationRecipient>> GetRecipientsForDeviceAsync(ListRequest listRequest, string deviceRepoId, string deviceId);
        Task<ListResponse<DeviceNotificationRecipient>> GetRecipientsForDeviceAndNotificationKeyAsync(ListRequest listRequest, string deviceRepoId, string deviceId, string notitificationKey);
        Task DeleteNotificationRecipientAsync(string deviceRepoId, string notificationRecipientId);
    }
}
