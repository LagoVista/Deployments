// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 99cb9e073ab5ad6f3f155dd052ea9e0213aa9c5a520e9b5b6428d710db952981
// IndexVersion: 2
// --- END CODE INDEX META ---
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

        public Task<ListResponse<RaisedNotificationHistory>> GetHistoryAsync(string deviceUniqueId, ListRequest listRequest)
        {

            return this.GetPagedResultsAsync(listRequest, FilterOptions.Create(nameof(DeviceNotificationHistory.DeviceUniqueId), FilterOptions.Operators.Equals, deviceUniqueId));
        }

        public Task<ListResponse<RaisedNotificationHistory>> GetHistoryForRepoAsync(string repoId, ListRequest listRequest)
        {
            return this.GetPagedResultsAsync(repoId, listRequest);
        }

        public Task<RaisedNotificationHistory> GetRaisedNotificationHistoryAsync(string rowKey, string paritionKey)
        {
            return GetAsync(paritionKey, rowKey);
        }
    }
}
