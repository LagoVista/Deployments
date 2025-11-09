// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bef27b589e6f104fc3017147ace2a57960558a6da26733d19b58e28becdb4439
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeploymentHostStatusRepo : TableStorageBase<DeploymentHostStatus>, IDeploymentHostStatusRepo
    {
        public DeploymentHostStatusRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger)
            : base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {
        }

        public Task AddDeploymentHostStatusAsync(DeploymentHostStatus hostStatus)
        {
            return InsertAsync(hostStatus);
        }

        public async Task<ListResponse<DeploymentHostStatus>> GetStatusHistoryForHostAsync(string hostId, ListRequest listRequest)
        {
            var response = await GetByParitionIdAsync(hostId);
            return ListResponse<DeploymentHostStatus>.Create(response);
        }
    }
}
