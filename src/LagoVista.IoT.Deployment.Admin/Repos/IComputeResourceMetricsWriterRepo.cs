using LagoVista.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    /// <summary>
    /// Appends host health samples to the shared Cassandra history store.
    /// </summary>
    public interface IComputeResourceMetricsWriterRepo
    {
        Task AddMetricAsync(ComputeResourceMetrics metrics, EntityHeader organization);
    }
}
