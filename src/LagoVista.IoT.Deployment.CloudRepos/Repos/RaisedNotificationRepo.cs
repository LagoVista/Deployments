using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class RaisedNotificationHistoryRepo : TableStorageBase<RaisedNotificationHistory>, IRaisedNotificationHistoryRepo
    {
        public RaisedNotificationHistoryRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }

        public Task AddHistoryAsync(RaisedNotificationHistory history)
        {
            return InsertAsync(history);
        }

        public Task<ListResponse<RaisedNotificationHistory>> GetHistoryAsync(string deviceId, ListRequest listRequest)
        {
            return this.GetPagedResultsAsync(deviceId, listRequest);
        }

        public Task<ListResponse<RaisedNotificationHistory>> GetHistoryForRepoAsync(string repoId, ListRequest listRequest)
        {
            return this.GetPagedResultsAsync(listRequest, FilterOptions.Create(nameof(DeviceNotificationHistory.DeviceRepoId), FilterOptions.Operators.Equals, repoId));
        }
    }
}
