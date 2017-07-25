using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    
        public Task<IEnumerable<DeploymentActivity>> GetFailedDeploymentActivitiesForResourceIdAsync(string resourceId)
        {
            return GetByParitionIdAsync(resourceId);
        }
    }
}
