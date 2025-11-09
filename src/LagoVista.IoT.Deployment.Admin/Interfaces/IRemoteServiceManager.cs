// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 62b5e123c6f53124d8fc97991784f1af5d9e8f2546e7a5f69e2ceb7cf662ed66
// IndexVersion: 2
// --- END CODE INDEX META ---
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

        Task<InvokeResult> ProvisionInstanceAsync(string orgId, string hostId, string instanceId, InstanceService service, List<InstanceAccount> instanceAccounts);
        Task<InvokeResult> RemoveInstanceAsync(string orgId, string hostId, string instanceId);


        Task<InvokeResult> RemoteInstanceStartingAsync(string orgId, string hostId, string instanceId);
        Task<InvokeResult> RemoteInstancePausingAsync(string orgId, string hostId, string instanceId);
        Task<InvokeResult> RemoteInstanceStoppingAsync(string orgId, string hostId, string instanceId);

        Task<InvokeResult> AddInstanceAccountAsync(string orgId, string hostId, string instanceId, InstanceAccount account);
        Task<InvokeResult> UpdateInstanceAccountAsync(string orgId, string hostId, string instanceId, InstanceAccount account);
        Task<InvokeResult> RemoveInstanceAccountAsync(string orgId, string hostId, string instanceId, string id);
    }
}
