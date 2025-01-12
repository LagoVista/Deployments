using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceNotificationTracking : TableStorageBase<DeviceNotificationHistory>, IDeviceNotificationTracking
    {
        public DeviceNotificationTracking(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }
     
        public Task AddHistoryAsync(DeviceNotificationHistory history)
        {
            return InsertAsync(history);
        }

        public async Task<ListResponse<DeviceNotificationHistory>> GetHistoryAsync(string deviceId, ListRequest listRequest)
        {
           var results = await this.GetPagedResultsAsync(deviceId, listRequest);
            results.Model = results.Model.OrderByDescending(fld=>fld.SentTimeStamp).ToList();
            return results;
        }

        public Task<DeviceNotificationHistory> GetHistoryAsync(string deviceId, string rowkey)
        {
            return GetAsync(deviceId, rowkey);
        }

        public Task<ListResponse<DeviceNotificationHistory>> GetHistoryForRepoAsync(string repoId, ListRequest listRequest)
        {
            return this.GetPagedResultsAsync(listRequest, FilterOptions.Create(nameof(DeviceNotificationHistory.DeviceRepoId), FilterOptions.Operators.Equals, repoId));
        }

        public async Task MarkAsViewed(string staticPageId)
        {
            var history = await this.GetAsync(staticPageId);
            if (!history.Viewed)
            {
                history.ViewedTimeStamp = DateTime.UtcNow.ToJSONString();
                history.Viewed = true;
                await UpdateAsync(history);
            }
        }
    }
}
