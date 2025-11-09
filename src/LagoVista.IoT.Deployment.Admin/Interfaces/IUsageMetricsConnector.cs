// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f50e598aacc0fc4436bf90d4280fd7d4dd8cc5906a9c27fd40e3a3eec1f7c6bb
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    interface IUsageMetricsConnector
    {
        Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request, EntityHeader org, EntityHeader user);
        Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, ListRequest request, EntityHeader org, EntityHeader user);
        Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user);
    }

}
