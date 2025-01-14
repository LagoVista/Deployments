using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class UsageMetricsRepo : TableStorageBase<UsageMetrics>, IUsageMetricsRepo
    {   
        public UsageMetricsRepo(IUsageMetricsRepoSettings repoSettings, IAdminLogger logger) 
            : base(repoSettings.UsageMetricsTableStorage.AccountId, repoSettings.UsageMetricsTableStorage.AccessKey, logger)
        {

        }

        public override StoragePeriod GetStoragePeriod()
        {
            return StoragePeriod.Month;
        }

        public Task<ListResponse<UsageMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request)
        {
            return GetPagedResultsAsync(hostId, request);
        }

        public Task<ListResponse<UsageMetrics>> GetMetricsForDependencyAsync(string dependencyId, ListRequest request)
        {
            // TODO: This will only get the metrics for the current month.  At the transitoin between months we will not have this available.
            return GetPagedResultsAsync(dependencyId, request);
        }

        public Task<ListResponse<UsageMetrics>> GetMetricsForInstanceAsync(string instanceId, ListRequest request)
        {
            return GetPagedResultsAsync(instanceId, request);
        }

        public Task<ListResponse<UsageMetrics>> GetMetricsForPipelineModuleAsync(string pipelineModuleId, ListRequest request)
        {
            return GetPagedResultsAsync(pipelineModuleId, request);
        }
    }
}
