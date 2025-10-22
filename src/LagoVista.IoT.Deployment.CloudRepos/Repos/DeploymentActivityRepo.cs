// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8d8193b670ab9920f9a8b96d5e2a2bd46815d4f6d6c11009ab3e1e4ae1ac53a3
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Repos;
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using LagoVista.Core;
using System;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentActivityRepo :  TableStorageBase<DeploymentActivity>, IDeploymentActivityRepo
    {
        public DeploymentActivityRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) : 
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }
       
        public Task AddDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return InsertAsync(deploymentActivity);
        }

        public Task<IEnumerable<DeploymentActivity>> GetScheduledDeploymentActivitiesAsync()
        {
            return GetByFilterAsync(FilterOptions.Create(nameof(DeploymentActivity.Status), FilterOptions.Operators.Equals, DeploymentActivityStatus.Scheduled.ToString()),
                                    FilterOptions.Create(nameof(DeploymentActivity.ScheduledFor), FilterOptions.Operators.LessThan, DateTime.UtcNow.ToJSONString()));
        }

        public Task<IEnumerable<DeploymentActivity>> GetRunningDeploymentActivitiesAsync()
        {
            return GetByFilterAsync(FilterOptions.Create(nameof(DeploymentActivity.Status), FilterOptions.Operators.Equals, DeploymentActivityStatus.Running.ToString()));
        }

        public Task<IEnumerable<DeploymentActivity>> GetRetryDeploymentActivitiesAsync()
        {
            return GetByFilterAsync(FilterOptions.Create(nameof(DeploymentActivity.Status), FilterOptions.Operators.Equals, DeploymentActivityStatus.PendingRetry.ToString()));
        }

        public Task<DeploymentActivity> GetDeploymentActivityAsync(string deploymentActivityId)
        {
            return GetAsync(deploymentActivityId);
        }

        public async Task<IEnumerable<DeploymentActivitySummary>> GetForResourceIdAsync(string resourceId)
        {
            var records = await GetByParitionIdAsync(resourceId);
            return from rec in records select rec.CreateSummary();
        }

        public Task RemoveDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return RemoveAsync(deploymentActivity, "*");
        }

        public async Task UpdateDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            var activity = await GetAsync(deploymentActivity.PartitionKey, deploymentActivity.RowKey);
            await UpdateAsync(deploymentActivity, "*");
        }
    }
}
