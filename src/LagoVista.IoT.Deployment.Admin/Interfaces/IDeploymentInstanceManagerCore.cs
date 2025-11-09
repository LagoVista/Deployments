// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b914e93798e7e0d85c6d5a55acb1f1eebeabb066a197bea262f6eb2b612a28b0
// IndexVersion: 2
// --- END CODE INDEX META ---
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
        Task<InvokeResult<DeploymentInstance>> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<DeploymentInstance>> GetInstanceAsync(string instanceId);
        Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org);
        Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance,  EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateInstanceStatusAsync(string instanceId, DeploymentInstanceStates newStatus, bool deployed, string version, EntityHeader org, EntityHeader user, string details = "");
    }
}