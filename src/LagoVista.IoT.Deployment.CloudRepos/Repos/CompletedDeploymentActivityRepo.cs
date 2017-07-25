using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class CompletedDeploymentActivityRepo : TableStorageBase<DeploymentActivity>, ICompletedDeploymentActivityRepo
    {
        public CompletedDeploymentActivityRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) : 
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }

        public Task AddCompletedDeploymentActivitiesAsync(DeploymentActivity deploymentActivity)
        {
            return InsertAsync(deploymentActivity);
        }

        public Task<IEnumerable<DeploymentActivity>> GetCompletedDeploymentActivitiesForResourceIdAsync(string resourceId)
        {
            return GetByParitionIdAsync(resourceId);
        }
    }
}
