using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class InstanceAccountRepo : TableStorageBase<InstanceAccountDTO>, IInstanceAccountsRepo
    {
        public InstanceAccountRepo(IDeploymentRepoSettings repoSettings, IAdminLogger adminLogger) :
            base(repoSettings.DeploymentAdminTableStorage.AccountId, repoSettings.DeploymentAdminTableStorage.AccessKey, adminLogger)
        {

        }

        public Task AddInstanceAccountAsync(string instanceId, InstanceAccount account)
        {
            return InsertAsync(account.CreateDTO(instanceId));
        }

        public async Task<bool> DoesUserNameExistsAsync(string instanceId, string userName)
        {
            var existing = await GetAsync(instanceId, userName, false);
            return existing != null;
        }

        public async Task<InstanceAccount> GetInstanceAccountAsync(string instanceId, string id)
        {
            var account = await GetAsync(instanceId, id);
            return account.ToInstanceAccount();
        }

        public async Task<List<InstanceAccount>> GetInstanceAccountsAsync(string instanceId)
        {
            var accounts = await GetByParitionIdAsync(instanceId);
            return accounts.Select(act => act.ToInstanceAccount()).OrderBy(act => act.UserName).ToList();
        }

        public async Task RemoveInstanceAccountAsync(string instanceId, string userName)
        {
            await RemoveAsync(instanceId, userName);
        }

        public Task UpdateInstanceAccountAsync(string instanceId, InstanceAccount account)
        {
            return UpdateAsync(account.CreateDTO(instanceId));
        }
    }
}
