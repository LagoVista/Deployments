// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1bad5ce6530db7794e89999601594172d25f809481e95bfd46af7a1c1308a97d
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentHostManagerRemote
    {
        Task<DeploymentHost> GetDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user, bool throwOnNotFound = true);
        Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeploymentInstanceSummary>> GetInstancesForHostAsync(String hostId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeploymentHostStatusAsync(string hostId, HostStatus hostStatus, string version, EntityHeader org, EntityHeader user, string details = "");
        Task<DeploymentHost> GetDeploymentHostAsync(string hostId);
    }

    public interface IDeploymentHostManager : IDeploymentHostManagerRemote
    {
        Task<InvokeResult> AddDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetNotificationsHostAsync(EntityHeader org, EntityHeader user);
        Task<DeploymentHost> GetMCPHostAsync(EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DeploymentHostSummary>> GetDeploymentHostsForOrgAsync(string orgId, EntityHeader user);
        Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org);
        

        Task<InvokeResult<string>> RegenerateKeyAsync(string hostId, string keyName, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeployHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeployContainerAsync(string hostId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartHostAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DestroyHostAsync(string id, EntityHeader org, EntityHeader user);

        Task<DeploymentHost> GetSecureDeploymentHostAsync(string hostId, EntityHeader org, EntityHeader user, bool throwOnNotFound = true);

        Task<IEnumerable<DeploymentActivitySummary>> GetHostActivitesAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<InstanceRuntimeSummary>> GetDeployedInstancesAsync(string hostId, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeploymentHostStatus>> GetDeploymentHostStatusHistoryAsync(string hostId, EntityHeader org, EntityHeader user, ListRequest listRequest);
        Task<DeploymentHost> FindHostServiceAsync(HostTypes hostType);

        Task<ListResponse<DeploymentHostSummary>> SysAdminGetActiveHostsAsync(EntityHeader org, EntityHeader user, ListRequest listRequest);
        Task<ListResponse<DeploymentHostSummary>> SysAdminFailedHostsAsync(EntityHeader org, EntityHeader user, ListRequest listRequest);
        Task<ListResponse<DeploymentHostSummary>> SysAdminAllHostsAsync(EntityHeader org, EntityHeader user, ListRequest listRequest);
    }
}
