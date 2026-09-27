// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5ef9752c11588ed67e31868dd2e65e3ca33b1b3eb443e15b1e9234df24fd0b61
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class UsageMetricsRepo : TableStorageBase<UsageMetrics>, IUsageMetricsRepo
    {
        private static readonly TimeSpan UsageRetention = TimeSpan.FromDays(90);
        private readonly IActivityRecordStore<UsageMetricActivityRecord> _activityStore;

        public UsageMetricsRepo(
            IUsageMetricsRepoSettings repoSettings,
            IAdminLogger logger,
            IActivityRecordStore<UsageMetricActivityRecord> activityStore)
            : base(repoSettings.UsageMetricsTableStorage.AccountId, repoSettings.UsageMetricsTableStorage.AccessKey, logger)
        {
            _activityStore = activityStore ?? throw new ArgumentNullException(nameof(activityStore));
        }

        public static void ConfigureStorage(StorageDefinition<UsageMetricActivityRecord> definition)
        {
            definition
                .KeyBy(record => record.Id)
                .PartitionBy(record => record.OrganizationId)
                .TimeBy(record => record.CreationDate)
                .BucketBy(StoragePeriod.Month)
                .Index(record => record.SourceId)
                .Index(record => record.HostId)
                .Index(record => record.InstanceId)
                .Index(record => record.PipelineModuleId)
                .RetainFor(UsageRetention);
        }

        public async Task AddMetricsAsync(IEnumerable<UsageMetrics> metrics, EntityHeader organization, string instanceId)
        {
            if (metrics == null) throw new ArgumentNullException(nameof(metrics));
            if (EntityHeader.IsNullOrEmpty(organization)) throw new ArgumentNullException(nameof(organization));
            if (String.IsNullOrWhiteSpace(instanceId)) throw new ArgumentNullException(nameof(instanceId));

            var records = metrics.Select(metric => ToActivityRecord(metric, organization, instanceId)).ToList();
            if (records.Count == 0) return;

            await _activityStore.InsertBatchAsync(records).ConfigureAwait(false);
        }

        private static UsageMetricActivityRecord ToActivityRecord(UsageMetrics metric, EntityHeader organization, string instanceId)
        {
            if (metric == null) throw new ArgumentNullException(nameof(metric));

            var creationDate = ParseCreationDate(metric.EndTimeStamp);
            var sourceId = FirstValue(metric.PartitionKey, metric.PipelineModuleId, instanceId, metric.HostId, "usage");
            var sourceRecordId = FirstValue(metric.RowKey, metric.EndTimeStamp, creationDate.Ticks.ToString(CultureInfo.InvariantCulture));

            return new UsageMetricActivityRecord
            {
                Id = $"{sourceId}:{sourceRecordId}",
                OrganizationId = organization.Id,
                Organization = organization.Text,
                CreationDate = creationDate,
                SourceId = sourceId,
                HostId = metric.HostId,
                InstanceId = instanceId,
                PipelineModuleId = metric.PipelineModuleId,
                StartTimeStamp = metric.StartTimeStamp,
                EndTimeStamp = metric.EndTimeStamp,
                ElapsedMS = metric.ElapsedMS,
                MessagesPerSecond = metric.MessagesPerSecond,
                AverageProcessingMS = metric.AverageProcessingMS,
                Version = metric.Version,
                Status = metric.Status,
                MessagesProcessed = metric.MessagesProcessed,
                DeadLetterCount = metric.DeadLetterCount,
                BytesProcessed = metric.BytesProcessed,
                ErrorCount = metric.ErrorCount,
                WarningCount = metric.WarningCount,
                ActiveCount = metric.ActiveCount,
                ProcessingMS = metric.ProcessingMS
            };
        }

        private static DateTime ParseCreationDate(string value)
        {
            if (!String.IsNullOrWhiteSpace(value) &&
                DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            {
                if (parsed.Kind == DateTimeKind.Utc) return parsed;
                if (parsed.Kind == DateTimeKind.Unspecified) return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);
                return parsed.ToUniversalTime();
            }

            return DateTime.UtcNow;
        }

        private static string FirstValue(params string[] values)
        {
            return values.First(value => !String.IsNullOrWhiteSpace(value));
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
            // Historical reads remain on Azure Table Storage until the separate data-migration/read-cutover work is scheduled.
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
