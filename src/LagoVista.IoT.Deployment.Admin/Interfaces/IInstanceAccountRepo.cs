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
