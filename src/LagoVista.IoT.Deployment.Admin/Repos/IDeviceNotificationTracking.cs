// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b35ad8ea83f8fef979622ad4d53ee05e8fed546effc20132910062162113a863
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeviceNotificationTracking
    {
        Task<ListResponse<DeviceNotificationHistory>> GetHistoryAsync(string deviceId, ListRequest listRequest);
        Task<ListResponse<DeviceNotificationHistory>> GetHistoryForRepoAsync(string repoId, ListRequest listRequest);
        Task<List<DeviceNotificationHistory>> GetHistoryForRaisedNotification(string raisedNotificationId, string deviceId);

        Task<DeviceNotificationHistory> GetHistoryAsync(string deviceId, string rowkey);

        Task AddHistoryAsync(DeviceNotificationHistory history);

        Task MarkAsViewed(string staticPageId);
    }
}
