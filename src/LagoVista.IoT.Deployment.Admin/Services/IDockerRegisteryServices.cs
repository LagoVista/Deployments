using System.Threading.Tasks;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public interface IDockerRegisteryServices
    {
        Task<InvokeResult<DockerRepoResponse>> GetReposAsync(string nameSpace, string token);
        Task<InvokeResult<DockerTagsResponse>> GetTagsAsync(string nameSpace, string repo, string token);
        Task<InvokeResult<string>> GetTokenAsync(string uid, string pwd);
    }
}