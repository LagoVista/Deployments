using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IRemoteDeploymentRepo
    {
        Task AddRemoteDeploymentAsync(RemoteDeployment deployment);
        Task<RemoteDeployment> GetRemoteDeploymentAsync(string id, bool populateChildren = false);
        Task<ListResponse<RemoteDeploymentSummary>> GetRemoteDeploymentsForOrgsAsync(string orgId);
        Task UpdateRemoteeDeploymentAsync(RemoteDeployment deployment);
        Task DeleteRemoteDeploymentAsync(string id);
        Task<bool> QueryKeyInUseAsync(string key, string org);
    }
}
