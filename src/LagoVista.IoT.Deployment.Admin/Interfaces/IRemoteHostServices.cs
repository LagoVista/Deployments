// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: daa37072bbc649d7a52c51400ead6f90853fac59034f75d530ac016f16afb688
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IRemoteHostServices
    {
        Task<InvokeResult> StartAsync();
        Task<InvokeResult> RestartAsync();
        Task<InvokeResult> StopAsync();
        Task<InvokeResult> InstanceStatusChangedAsync(string instanceId, string newState);

        Task<InvokeResult> ProvisionInstanceAsync(string instanceId, InstanceService service, List<InstanceAccount> accounts);
        Task<InvokeResult> RemoveInstanceAsync(string instanceId);

        Task<InvokeResult> AddInstanceAccountAsync(string instanceId, InstanceAccount account);
        Task<InvokeResult> UpdateInstanceAccountAsync(string instanceId, InstanceAccount account);
        Task<InvokeResult> RemoveInstanceAccountAsync(string instanceId, string id);


    }
}
