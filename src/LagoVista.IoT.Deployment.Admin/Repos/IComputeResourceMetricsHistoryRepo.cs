using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    /// <summary>
    /// Read-only host compute metrics history. Callers must authorize access to the host.
    /// </summary>
    public interface IComputeResourceMetricsHistoryRepo
    {
        Task<ListResponse<ComputeResourceMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request);
    }
}
