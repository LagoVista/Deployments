using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IRemoteDeploymentManager
    {
        Task<InvokeResult> AddRemoteDeploymentAsync(RemoteDeployment host, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateRemoteDeploymentAsync(RemoteDeployment host, EntityHeader org, EntityHeader user);
        Task<RemoteDeployment> GetRemoteDeploymentAsync(string id, EntityHeader org, EntityHeader user);
        Task<RemoteDeployment> LoadFullRemoteDeploymentAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<RemoteDeploymentSummary>> GetRemoteDeploymentsForOrgAsync(string orgId, EntityHeader user);
        Task<InvokeResult> DeleteRemoteDeploymentHostAsync(String instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RegenerateAccessKeys(string id, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);
        Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org);
    }
}
