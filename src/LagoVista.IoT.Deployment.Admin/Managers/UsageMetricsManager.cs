using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Models;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class UsageMetricsManager : ManagerBase, IUsageMetricsManager
    {
        IUsageMetricsRepo _metricsRepo;

        public UsageMetricsManager(IAdminLogger adminLogger, IAppConfig appConfig, IUsageMetricsRepo metricsRepo, IDependencyManager dependencyManager, ISecurity security) : base(adminLogger, appConfig, dependencyManager, security)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForDependencyAsync(string dependencyId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "Dependency");
            return await _metricsRepo.GetMetricsForDependencyAsync(dependencyId, request);
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "Host");
            return await  _metricsRepo.GetMetricsForHostAsync(hostId, request);
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "Instance");
            return await _metricsRepo.GetMetricsForInstanceAsync(instanceId, request);
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "PipelineModule");
            return await _metricsRepo.GetMetricsForPipelineModuleAsync(pipelineModuleId, request);
        }
    }
}
