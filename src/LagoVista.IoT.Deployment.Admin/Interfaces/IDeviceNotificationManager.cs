// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8587a750538f95d519cd51497254d3e6f8f408150d1c787bb36b4c8c1dadba54
// IndexVersion: 0
// --- END CODE INDEX META ---
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
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationTemplatesForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<ListResponse<DeviceNotificationSummary>> GetNotificationsForCustomerAsync(string orgId, string customerId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateNotificationAsync(DeviceNotification Notification, EntityHeader org, EntityHeader user);
        Task<InvokeResult> AcknowledgeNotificationAsync(string notifid, string recipientid);
        Task<ListResponse<DeviceNotificationHistory>> GetNotificationHistoryAsync(string deviceid, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceNotificationHistory>> GetNotificationHistoryForRepoAsync(string repoId, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<ListResponse<RaisedNotificationHistory>> GetRasiedNotificationHistoryAsync(string deviceid, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<InvokeResult<RaiseNotificationSummary>> GetRasiedNotificationSummaryAsync(string repoId, string rowKey, EntityHeader org, EntityHeader user);
        Task<ListResponse<RaisedNotificationHistory>> GetRaisedNotificationHistoryForRepoAsync(string repoId, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
