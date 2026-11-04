using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public sealed class CompletedDeploymentActivityRepo : ICompletedDeploymentActivityRepo
    {
        private readonly IActivityRecordStore<CompletedDeploymentActivityRecord> _store;

        public CompletedDeploymentActivityRepo(IActivityRecordStore<CompletedDeploymentActivityRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Task AddCompletedDeploymentActivitiesAsync(DeploymentActivity activity)
        {
            return _store.InsertAsync(CompletedDeploymentActivityRecord.From(activity));
        }

        public async Task<IEnumerable<DeploymentActivitySummary>> GetCompletedDeploymentActivitiesForResourceIdAsync(string resourceId)
        {
            if (String.IsNullOrWhiteSpace(resourceId)) throw new ArgumentNullException(nameof(resourceId));
            var page = await _store.QueryAsync(new HistoryQuery<CompletedDeploymentActivityRecord>()
                .Where(record => record.ResourceId, StorageFilterOperator.Equal, resourceId)
                .WithPage(new StoragePageRequest(1000))).ConfigureAwait(false);
            // Existing API returns all activities; keep retrieving the full resource
            // history until this contract exposes first-class continuation tokens.
            var history = new List<CompletedDeploymentActivityRecord>();
            while (true)
            {
                history.AddRange(page.Items);
                if (!page.HasMoreRecords) return history.OrderByDescending(record => record.CreationDate).Select(record => record.CreateSummary()).ToList();
                page = await _store.QueryAsync(new HistoryQuery<CompletedDeploymentActivityRecord>()
                    .Where(record => record.ResourceId, StorageFilterOperator.Equal, resourceId)
                    .WithPage(new StoragePageRequest(1000, page.ContinuationToken))).ConfigureAwait(false);
            }
        }
    }
}
