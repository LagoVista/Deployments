using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public sealed class UsageMetricsBatchRequest
    {
        public List<UsageMetrics> Metrics { get; set; } = new List<UsageMetrics>();
    }
}
