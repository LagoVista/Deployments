using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Repos;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IDeviceNotificationManager
    {
        Task<InvokeResult<string>> HandleNotificationAsync(string notifid, string orgid, string recipientid, string pageid);

        Task<InvokeResult> AddNotificationAsync(DeviceNotification Notification, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteNotificationAsync(string id, EntityHeader org, EntityHeader user);
        Task<DeviceNotification> GetNotificationAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationTempmlatesForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationsForCustomerAsync(string orgId, string customerId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateNotificationAsync(DeviceNotification Notification, EntityHeader org, EntityHeader user);
        Task<InvokeResult> AcknowledgeNotificationAsync(string notifid, string recipientid);
        Task<ListResponse<DeviceNotificationHistory>> GetNotificationHistoryAsync(string deviceid, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceNotificationHistory>> GetNotificationHistoryForRepoAsync(string repoId, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<ListResponse<RaisedNotificationHistory>> GetRasiedNotificationHistoryAsync(string deviceid, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<ListResponse<RaisedNotificationHistory>> GetRaisedNotificationHistoryForRepoAsync(string repoId, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
