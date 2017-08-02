using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class CompletedDeploymentActivityRepo : TableStorageBase<DeploymentActivity>, ICompletedDeploymentActivityRepo
    {
        public CompletedDeploymentActivityRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) : 
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }

        protected override string GetTableName()
        {
            return "CompletedDeploymentActivity";
        }

        public Task AddCompletedDeploymentActivitiesAsync(DeploymentActivity deploymentActivity)
        {
            return InsertAsync(deploymentActivity);
        }

        public async Task<IEnumerable<DeploymentActivitySummary>> GetCompletedDeploymentActivitiesForResourceIdAsync(string resourceId)
        {
            var records = await GetByParitionIdAsync(resourceId);
            return from rec in records select rec.CreateSummary();
        }
    }
}
