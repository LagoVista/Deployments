// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fd694db4497eec077bd39238f558e98a718bf2282ae66d021d52ed68f444d631
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Deployment.Admin.Repos;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public class UsageMetricsConnector : ConnectorServiceBase, IUsageMetricsConnector
    {
        public UsageMetricsConnector(IDeploymentHostRepo deploymentHostRepo, IAdminLogger logger) : base(deploymentHostRepo, logger)
        {

        }

        public Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }
    }
}
