using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public sealed class FailedDeploymentActivityRepo : IFailedDeploymentActivityRepo
    {
        private readonly IActivityRecordStore<FailedDeploymentActivityRecord> _store;

        public FailedDeploymentActivityRepo(IActivityRecordStore<FailedDeploymentActivityRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Task AddFailedDeploymentActivityAsync(DeploymentActivity activity)
        {
            return _store.InsertAsync(FailedDeploymentActivityRecord.From(activity));
        }

        public async Task<IEnumerable<DeploymentActivitySummary>> GetFailedDeploymentActivitiesForResourceIdAsync(string resourceId)
        {
            if (String.IsNullOrWhiteSpace(resourceId)) throw new ArgumentNullException(nameof(resourceId));
            var history = new List<FailedDeploymentActivityRecord>();
            string cursor = null;
            do
            {
                var page = await _store.QueryAsync(new HistoryQuery<FailedDeploymentActivityRecord>()
                    .Where(record => record.ResourceId, StorageFilterOperator.Equal, resourceId)
                    .WithPage(new StoragePageRequest(1000, cursor))).ConfigureAwait(false);
                history.AddRange(page.Items);
                cursor = page.ContinuationToken;
            } while (!String.IsNullOrWhiteSpace(cursor));
            return history.OrderByDescending(record => record.CreationDate).Select(record => record.CreateSummary()).ToList();
        }

        public Task RemoveFailedDeploymentActivityAsync(DeploymentActivity activity)
        {
            // Terminal deployment history is append-only. No production callers
            // remain for the legacy Table Storage delete operation.
            throw new NotSupportedException("Failed deployment history is immutable in Cassandra; deletion is not supported.");
        }
    }
}
