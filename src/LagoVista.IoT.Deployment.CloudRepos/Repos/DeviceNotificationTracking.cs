﻿using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
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

        public Task<ListResponse<DeviceNotificationHistory>> GetHistoryAsync(string deviceId, ListRequest listRequest)
        {
            return this.GetPagedResultsAsync(deviceId, listRequest);
        }

        public async Task MarkAsViewed(string staticPageId)
        {
            var history = await this.GetAsync(staticPageId);
            history.ViewedTimeStamp = DateTime.UtcNow.ToJSONString();
            history.Viewed = true;
            await UpdateAsync(history);
        }
    }
}
