// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2fbc4afa1c19e21f0d7754bc3008cc11cc424f5ff907135ae2b95440cf253a8d
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections.Generic;
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

        public async Task<List<DeviceNotificationHistory>> GetHistoryForRaisedNotification(string raisedNotificationId, string deviceId)
        {
            var result = await this.GetPagedResultsAsync(deviceId, ListRequest.CreateForAll(), FilterOptions.Create(nameof(DeviceNotificationHistory.RaisedNotificationId), FilterOptions.Operators.Equals, raisedNotificationId));
            return result.Model.ToList();
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
