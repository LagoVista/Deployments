// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d23c7cfec19a0fd5e35b802534bdaf12699f13d1237ca327cd5003f9c09d0276
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Models.UIMetaData;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Deployment.Admin.Repos;

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


        public Task<ListResponse<DeploymentInstanceStatus>> GetStatusHistoryForInstanceAsync(string instanceId, ListRequest listRequest)
        {
            return GetPagedResultsAsync(instanceId, listRequest);
        }
    }
}
