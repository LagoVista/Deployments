using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDeploymentInstanceManagerCore
    {
        Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<DeploymentInstance> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<DeploymentInstance> GetInstanceAsync(string instanceId);
        Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org);
        Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance,  EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateInstanceStatusAsync(string instanceId, DeploymentInstanceStates newStatus, bool deployed, string version, EntityHeader org, EntityHeader user, string details = "");
    }
}