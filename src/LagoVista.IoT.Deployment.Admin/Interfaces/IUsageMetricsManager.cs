// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4296d92dfb97df3c545e275a0a4a1a11a3746aa8194da5f4f07f98005f1dc713
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IUsageMetricsManager
    {
        Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request, EntityHeader org, EntityHeader user);
        Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, ListRequest request, EntityHeader org, EntityHeader user);
        Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string instanceId, string pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user);
        Task<ListResponse<UsageMetrics>> GetMetricsForDependencyAsync(string instanceId, string dependencyId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
