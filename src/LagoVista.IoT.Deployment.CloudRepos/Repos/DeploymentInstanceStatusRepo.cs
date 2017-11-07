using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core.Models.UIMetaData;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentInstanceStatusRepo : TableStorageBase<DeploymentInstanceStatus>, IDeploymentInstanceStatusRepo
    {
        public DeploymentInstanceStatusRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {
        }

        public Task AddDeploymentInstanceStatusAsync(DeploymentInstanceStatus instanceStatus)
        {
            return InsertAsync(instanceStatus);
        }

        public async Task<ListResponse<DeploymentInstanceStatus>> GetStatusHistoryForInstanceAsync(string instanceId, ListRequest listRequest)
        {
            var response = await GetByParitionIdAsync(instanceId);
            return ListResponse<DeploymentInstanceStatus>.Create(response);
        }
    }
}
