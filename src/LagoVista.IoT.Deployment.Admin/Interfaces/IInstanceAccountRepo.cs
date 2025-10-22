// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 672afe274d9f10610524b471e14af6ab8f0051775a7200eed10893e1cead09e8
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IInstanceAccountsRepo
    {
        Task AddInstanceAccountAsync(string instanceId, InstanceAccount account);
        Task UpdateInstanceAccountAsync(string instanceId, InstanceAccount account);
        Task RemoveInstanceAccountAsync(string instanceId, string instanceAccountId);
        Task<List<InstanceAccount>> GetInstanceAccountsAsync(string instanceId);
        Task<InstanceAccount> GetInstanceAccountAsync(string instanceId, string id);
        Task<bool> DoesUserNameExistsAsync(string instanceId, string userName);
    }
}
