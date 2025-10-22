// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b569edc1d1a0bac8a9531c9d9558fb0f427a8163de40f1fd8f3b37327f4904fb
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class FailedDeploymentActivityRepo : TableStorageBase<DeploymentActivity>, IFailedDeploymentActivityRepo
    {
        public FailedDeploymentActivityRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }

        public Task AddFailedDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return InsertAsync(deploymentActivity);
        }

        protected override string GetTableName()
        {
            return "FailedDeploymentActivity";
        }

        public Task RemoveFailedDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return RemoveAsync(deploymentActivity);
        }
    
        public async Task<IEnumerable<DeploymentActivitySummary>> GetFailedDeploymentActivitiesForResourceIdAsync(string resourceId)
        {
            var records = await GetByParitionIdAsync(resourceId);
            return from rec in records select rec.CreateSummary();
        }
    }
}
