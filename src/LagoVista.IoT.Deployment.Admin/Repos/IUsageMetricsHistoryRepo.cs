using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    /// <summary>
    /// Server-owned usage history reads. Organization scope is explicit because
    /// Cassandra activity records are partitioned by OrganizationId.
    /// </summary>
    public interface IUsageMetricsHistoryRepo
    {
        Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, EntityHeader organization, ListRequest request);
        Task<ListResponse<UsageMetrics>> GetMetricsForDependencyAsync(string dependencyId, EntityHeader organization, ListRequest request);
        Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, EntityHeader organization, ListRequest request);
        Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string pipelineModuleId, EntityHeader organization, ListRequest request);
    }
}
