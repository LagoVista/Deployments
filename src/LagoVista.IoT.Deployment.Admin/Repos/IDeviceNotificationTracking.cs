using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeviceNotificationTracking
    {
        Task<ListResponse<DeviceNotificationHistory>> GetHistoryAsync(string deviceId, ListRequest listRequest);
        Task<ListResponse<DeviceNotificationHistory>> GetHistoryForRepoAsync(string repoId, ListRequest listRequest);

        Task<DeviceNotificationHistory> GetHistoryAsync(string deviceId, string rowkey);

        Task AddHistoryAsync(DeviceNotificationHistory history);

        Task MarkAsViewed(string staticPageId);
    }
}
