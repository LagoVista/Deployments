// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 54ee6af4df0d04474364f912d0c3b74ffe7eb0b86613bc46bec342277ff5edcf
// IndexVersion: 0
// --- END CODE INDEX META ---
using System.Threading.Tasks;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IDockerRegisteryServices
    {
        Task<InvokeResult<DockerRepoResponse>> GetReposAsync(string nameSpace, string token);
        Task<InvokeResult<DockerTagsResponse>> GetTagsAsync(string nameSpace, string repo, string token);
        Task<InvokeResult<string>> GetTokenAsync(string uid, string pwd);
    }
}