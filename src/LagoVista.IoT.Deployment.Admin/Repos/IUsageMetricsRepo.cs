// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 929a05f3e875a8323829e50c6761e620751e1c00b7b1276c99c14ac52615024e
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IUsageMetricsRepo
    {
        Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request);
        Task<ListResponse<UsageMetrics>> GetMetricsForDependencyAsync(string dependencyId, ListRequest request);
        Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, ListRequest request);
        Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string pipelineModuleId, ListRequest request);
    }
}
