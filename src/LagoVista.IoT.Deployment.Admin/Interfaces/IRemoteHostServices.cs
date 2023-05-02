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

        Task<InvokeResult> ProvisionInstanceAsync(string instanceId, InstanceService service);
        Task<InvokeResult> RemoveInstanceAsync(string instanceId);

        Task<InvokeResult> AddInstanceAccountAsync(string instanceId, InstanceAccount account);
        Task<InvokeResult> UpdateInstanceAccountAsync(string instanceId, InstanceAccount account);
        Task<InvokeResult> RemoveInstanceAccountAsync(string instanceId, string id);


    }
}
