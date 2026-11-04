using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public sealed class DeploymentActivityRepo : IDeploymentActivityRepo
    {
        private readonly IConditionalOperationalDataStore<ActiveDeploymentActivityRecord> _store;

        public DeploymentActivityRepo(IConditionalOperationalDataStore<ActiveDeploymentActivityRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task AddDeploymentActivityAsync(DeploymentActivity activity)
        {
            var record = ActiveDeploymentActivityRecord.From(activity);
            var created = await _store.TryCreateAsync(record).ConfigureAwait(false);
            if (!created.Applied)
                throw new InvalidOperationException($"Deployment activity {record.Id} already exists in the Cassandra active queue; current status={created.Current?.Status ?? "unknown"}.");

            activity.StorageVersion = created.Current?.Version ?? record.Version;
        }

        public async Task UpdateDeploymentActivityAsync(DeploymentActivity activity)
        {
            var record = ActiveDeploymentActivityRecord.From(activity);
            if (activity.StorageVersion <= 0)
                throw new InvalidOperationException($"Cannot update deployment activity {record.Id}: missing Cassandra StorageVersion.");

            var updated = await _store.TryReplaceAsync(record, activity.StorageVersion).ConfigureAwait(false);
            if (!updated.Applied)
                throw new InvalidOperationException(
                    $"Concurrent deployment activity update rejected for {record.Id}: expected version={activity.StorageVersion}, current version={updated.Current?.Version}, current status={updated.Current?.Status}.");

            activity.StorageVersion = updated.Current?.Version ?? record.Version;
        }

        public async Task RemoveDeploymentActivityAsync(DeploymentActivity activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (String.IsNullOrWhiteSpace(activity.RowKey)) throw new ArgumentException("Deployment activity RowKey is required.", nameof(activity));
            if (activity.StorageVersion <= 0)
                throw new InvalidOperationException($"Cannot remove deployment activity {activity.RowKey}: missing Cassandra StorageVersion.");

            var removed = await _store.TryDeleteAsync(
                ActiveDeploymentActivityRecord.QueueScope, activity.RowKey, activity.StorageVersion).ConfigureAwait(false);
            if (!removed.Applied)
                throw new InvalidOperationException(
                    $"Concurrent deployment activity removal rejected for {activity.RowKey}: expected version={activity.StorageVersion}, current version={removed.Current?.Version}, current status={removed.Current?.Status}.");
        }

        public async Task<DeploymentActivity> GetDeploymentActivityAsync(string deploymentActivityId)
        {
            if (String.IsNullOrWhiteSpace(deploymentActivityId)) throw new ArgumentNullException(nameof(deploymentActivityId));
            var record = await _store.GetAsync(ActiveDeploymentActivityRecord.QueueScope, deploymentActivityId).ConfigureAwait(false);
            return record?.ToActivity();
        }

        public Task<IEnumerable<DeploymentActivity>> GetRunningDeploymentActivitiesAsync()
            => QueryAsync(DeploymentActivityStatus.Running);

        public Task<IEnumerable<DeploymentActivity>> GetRetryDeploymentActivitiesAsync()
            => QueryAsync(DeploymentActivityStatus.PendingRetry);

        public Task<IEnumerable<DeploymentActivity>> GetScheduledDeploymentActivitiesAsync()
            => QueryAsync(DeploymentActivityStatus.Scheduled, DateTime.UtcNow);

        public async Task<IEnumerable<DeploymentActivitySummary>> GetForResourceIdAsync(string resourceId)
        {
            if (String.IsNullOrWhiteSpace(resourceId)) throw new ArgumentNullException(nameof(resourceId));
            var activities = await QueryAllAsync(new StorageQuery<ActiveDeploymentActivityRecord>()
                .Where(record => record.OrganizationId, StorageFilterOperator.Equal, ActiveDeploymentActivityRecord.QueueScope)
                .Where(record => record.ResourceId, StorageFilterOperator.Equal, resourceId)).ConfigureAwait(false);
            return activities.Select(record => record.ToActivity().CreateSummary()).ToList();
        }

        private async Task<IEnumerable<DeploymentActivity>> QueryAsync(DeploymentActivityStatus status, DateTime? dueBefore = null)
        {
            var query = new StorageQuery<ActiveDeploymentActivityRecord>()
                .Where(record => record.OrganizationId, StorageFilterOperator.Equal, ActiveDeploymentActivityRecord.QueueScope)
                .Where(record => record.Status, StorageFilterOperator.Equal, status.ToString());

            // Cassandra SAI range query; this must never become an unbounded client-side
            // scan of Scheduled activities. Requires indexed range support in CloudStorage.
            if (dueBefore.HasValue)
                query.Where(record => record.ScheduledFor, StorageFilterOperator.LessThan, dueBefore.Value);

            var records = await QueryAllAsync(query).ConfigureAwait(false);
            return records.Select(record => record.ToActivity()).ToList();
        }

        private async Task<IReadOnlyList<ActiveDeploymentActivityRecord>> QueryAllAsync(StorageQuery<ActiveDeploymentActivityRecord> query)
        {
            var records = new List<ActiveDeploymentActivityRecord>();
            string cursor = null;
            do
            {
                query.WithPage(new StoragePageRequest(1000, cursor));
                var page = await _store.QueryAsync(query).ConfigureAwait(false);
                records.AddRange(page.Items);
                cursor = page.ContinuationToken;
            } while (!String.IsNullOrWhiteSpace(cursor));
            return records;
        }
    }
}
