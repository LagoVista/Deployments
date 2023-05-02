using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Models;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admins
{
    public interface IRemoteServiceManager
    {
        Task<InvokeResult> StartAsync(string orgId, string hostId);
        Task<InvokeResult> RestartAsync(string orgId, string hostId);
        Task<InvokeResult> StopAsync(string orgId, string hostId);

        Task<InvokeResult> ProvisionInstanceAsync(string orgId, string hostId, string instanceId, InstanceService service, IEnumerable<InstanceAccount> instanceAccounts);
        Task<InvokeResult> RemoveInstanceAsync(string orgId, string hostId, string instanceId);


        Task<InvokeResult> RemoteInstanceStartingAsync(string orgId, string hostId, string instanceId);
        Task<InvokeResult> RemoteInstancePausingAsync(string orgId, string hostId, string instanceId);
        Task<InvokeResult> RemoteInstanceStoppingAsync(string orgId, string hostId, string instanceId);

        Task<InvokeResult> AddInstanceAccountAsync(string orgId, string hostId, string instanceId, InstanceAccount account);
        Task<InvokeResult> UpdateInstanceAccountAsync(string orgId, string hostId, string instanceId, InstanceAccount account);
        Task<InvokeResult> RemoveInstanceAccountAsync(string orgId, string hostId, string instanceId, string id);
    }
}
