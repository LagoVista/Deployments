using LagoVista.IoT.Deployment.Admin.Repos;
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;

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

        public Task<List<DeploymentActivity>> GetForResourceIdAsync(string resourceId)
        {
            return GetForResourceIdAsync(resourceId);
        }

        public Task UpdateDeploymentActivityAsync(DeploymentActivity deploymentActivity)
        {
            return UpdateAsync(deploymentActivity);
        }
    }
}
