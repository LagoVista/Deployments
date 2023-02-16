using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class InstanceAccountRepo : IInstanceAccountsRepo
    {
        public Task<List<InstanceAccount>> GetInstanceAccountsAsync(string instanceId)
        {
            throw new NotImplementedException();
        }

        public Task StoreInstanceAccountsAsync(string instanceId, IEnumerable<InstanceAccount> accounts)
        {
            throw new NotImplementedException();
        }
    }
}
