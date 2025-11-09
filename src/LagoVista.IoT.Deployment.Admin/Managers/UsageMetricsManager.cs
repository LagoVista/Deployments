// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 54abf7bd43738cea8922fec0b0ea69af9ad9700f69d280fff03aea20832d400b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Rpc.Client;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class UsageMetricsManager : ManagerBase, IUsageMetricsManager
    {
        IUsageMetricsRepo _metricsRepo;
        IProxyFactory _proxyFactory;
        IDeploymentInstanceRepo _deploymentInstanceRepo;

        public UsageMetricsManager(IAdminLogger adminLogger, IAppConfig appConfig, IDeploymentInstanceRepo deploymentInstanceMgr,
            IProxyFactory proxyFactory, IUsageMetricsRepo metricsRepo, IDependencyManager dependencyManager, ISecurity security) :
            base(adminLogger, appConfig, dependencyManager, security)
        {
            _metricsRepo = metricsRepo;
            _deploymentInstanceRepo = deploymentInstanceMgr;
            _proxyFactory = proxyFactory;
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "Host");
            return await _metricsRepo.GetMetricsForHostAsync(hostId, request);
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);

            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "Instance");

            if (instance.LogStorage.Value == LogStorage.Local)
            {
                var proxy = _proxyFactory.Create<IUsageMetricsRepo>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetMetricsForInstanceAsync(instanceId, request);
            }
            else
            {
                return await _metricsRepo.GetMetricsForInstanceAsync(instanceId, request);
            }
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string instanceId, string pipelineModuleId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "PipelineModule");

            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);

            if (instance.LogStorage.Value == LogStorage.Local)
            {
                var proxy = _proxyFactory.Create<IUsageMetricsRepo>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetMetricsForPipelineModuleAsync(pipelineModuleId, request);
            }
            else
            {
                return await _metricsRepo.GetMetricsForPipelineModuleAsync(pipelineModuleId, request);
            }
        }

        public async Task<ListResponse<UsageMetrics>> GetMetricsForDependencyAsync(string instanceId, string dependencyId, ListRequest request, EntityHeader org, EntityHeader user)
        {
            var instance = await _deploymentInstanceRepo.GetInstanceAsync(instanceId);

            await AuthorizeOrgAccessAsync(user, org, typeof(UsageMetrics), Core.Validation.Actions.Read, "Dependency");

            if (instance.LogStorage.Value == LogStorage.Local)
            {
                var proxy = _proxyFactory.Create<IUsageMetricsRepo>(new ProxySettings { OrganizationId = org.Id, InstanceId = instanceId });
                return await proxy.GetMetricsForDependencyAsync(dependencyId, request);
            }
            else
            {
                return await _metricsRepo.GetMetricsForDependencyAsync(dependencyId, request);
            }
        }

    }
}
