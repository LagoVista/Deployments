using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    /// <summary>
    /// Cassandra-backed host health samples. The read surface is intentionally separate
    /// from the write surface; REST callers must authorize the requested host first.
    /// </summary>
    public sealed class ComputeResourceMetricsRepo : IComputeResourceMetricsWriterRepo, IComputeResourceMetricsHistoryRepo
    {
        private static readonly TimeSpan Retention = TimeSpan.FromDays(90);
        private readonly IActivityRecordStore<ComputeResourceMetricActivityRecord> _store;

        public ComputeResourceMetricsRepo(IActivityRecordStore<ComputeResourceMetricActivityRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public static void ConfigureStorage(StorageDefinition<ComputeResourceMetricActivityRecord> definition)
        {
            definition
                .KeyBy(record => record.Id)
                .PartitionBy(record => record.HostId)
                .TimeBy(record => record.CreationDate)
                .BucketBy(StoragePeriod.Month)
                .RetainFor(Retention);
        }

        public Task AddMetricAsync(ComputeResourceMetrics metrics, EntityHeader organization)
        {
            if (metrics == null) throw new ArgumentNullException(nameof(metrics));
            if (String.IsNullOrWhiteSpace(metrics.PartitionKey))
                throw new ArgumentException("Host id (PartitionKey) is required for compute resource metrics.", nameof(metrics));

            return _store.InsertAsync(new ComputeResourceMetricActivityRecord
            {
                Id = String.IsNullOrWhiteSpace(metrics.RowKey) ? Guid.NewGuid().ToString("N") : metrics.RowKey,
                HostId = metrics.PartitionKey,
                OrganizationId = organization?.Id,
                Organization = organization?.Text,
                CreationDate = ParseEndUtc(metrics.EndTimeStamp),
                StartTimeStamp = metrics.StartTimeStamp,
                EndTimeStamp = metrics.EndTimeStamp,
                ElapsedMS = metrics.ElapsedMS,
                Status = metrics.Status,
                StatusDetails = metrics.StatusDetails,
                CpuPercent = metrics.CpuPercent,
                MemoryPercent = metrics.MemoryPercent
            });
        }

        public async Task<ListResponse<ComputeResourceMetrics>> GetMetricsForHostAsync(string hostId, ListRequest request)
        {
            if (String.IsNullOrWhiteSpace(hostId))
                throw new ArgumentException("Host id is required.", nameof(hostId));

            request ??= ListRequest.Create(1, 100);
            if (!request.TryGetDateRange(out var startUtc, out var endExclusiveUtc, out var dateError))
                return ListResponse<ComputeResourceMetrics>.FromError(dateError);

            var endUtc = endExclusiveUtc.HasValue ? endExclusiveUtc.Value.AddTicks(-1) : DateTime.UtcNow;
            var start = startUtc ?? endUtc.Subtract(Retention);
            var pageSize = request.PageSize <= 0 ? 100 : Math.Min(request.PageSize, 1000);
            var continuationToken = String.Equals(request.NextPartitionKey, "cassandra", StringComparison.OrdinalIgnoreCase)
                ? request.NextRowKey
                : null;

            var query = new HistoryQuery<ComputeResourceMetricActivityRecord>()
                .Between(start, endUtc)
                .Where(record => record.HostId, StorageFilterOperator.Equal, hostId)
                .WithPage(new StoragePageRequest(pageSize, continuationToken));

            var page = await _store.QueryAsync(query).ConfigureAwait(false);
            return ListResponse<ComputeResourceMetrics>.Create(
                page.Items.Select(ToModel).ToList(),
                request,
                page.HasMoreRecords,
                page.HasMoreRecords ? "cassandra" : null,
                page.ContinuationToken);
        }

        private static ComputeResourceMetrics ToModel(ComputeResourceMetricActivityRecord record)
        {
            return new ComputeResourceMetrics
            {
                PartitionKey = record.HostId,
                RowKey = record.Id,
                StartTimeStamp = record.StartTimeStamp,
                EndTimeStamp = record.EndTimeStamp,
                ElapsedMS = record.ElapsedMS,
                Status = record.Status,
                StatusDetails = record.StatusDetails,
                CpuPercent = record.CpuPercent,
                MemoryPercent = record.MemoryPercent
            };
        }

        private static DateTime ParseEndUtc(string value)
        {
            const DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces |
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

            if (!String.IsNullOrWhiteSpace(value))
            {
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, styles, out var utc) ||
                    DateTime.TryParse(value, CultureInfo.CurrentCulture, styles, out utc))
                    return utc;
            }

            return DateTime.UtcNow;
        }
    }
}
