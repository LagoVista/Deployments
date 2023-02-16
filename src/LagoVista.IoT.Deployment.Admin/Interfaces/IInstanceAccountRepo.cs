using LagoVista.IoT.Deployment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IInstanceAccountsRepo
    {
        Task StoreInstanceAccountsAsync(string instanceId, IEnumerable<InstanceAccount> accounts);
        Task<List<InstanceAccount>> GetInstanceAccountsAsync(string instanceId);
    }
}
