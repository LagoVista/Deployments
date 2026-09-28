using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public sealed class RuntimeLogActivityRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsError { get; set; }
        public string LogLevel { get; set; }
        public string Application { get; set; }
        public string Environment { get; set; }
        public string ExceptionType { get; set; }
        public string HostId { get; set; }
        public string Version { get; set; }
        public string PemId { get; set; }
        public string InstanceId { get; set; }
        public string PipelineModuleId { get; set; }
        public string DeviceTypeId { get; set; }
        public string DeviceId { get; set; }
        public string ActivityId { get; set; }
        public string ErrorCode { get; set; }
        public string Area { get; set; }
        public string Tag { get; set; }
        public double Measure { get; set; }
        public double MS { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string StackTrace { get; set; }
        public string OldState { get; set; }
        public string NewState { get; set; }
        public string ParametersJson { get; set; }
    }

    public sealed class RuntimeLogRepo : IRuntimeLogRepo
    {
        private static readonly TimeSpan DefaultQueryWindow = TimeSpan.FromDays(90);
        private readonly IActivityRecordStore<RuntimeLogActivityRecord> _store;

        public RuntimeLogRepo(IActivityRecordStore<RuntimeLogActivityRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        internal static void ConfigureStorage(StorageDefinition<RuntimeLogActivityRecord> definition)
        {
            definition.PartitionBy(record => record.OrganizationId);
            definition.BucketBy(StoragePeriod.Month);
            definition.Index(record => record.IsError);
            definition.Index(record => record.LogLevel);
            definition.Index(record => record.Application);
            definition.Index(record => record.HostId);
            definition.Index(record => record.InstanceId);
            definition.Index(record => record.PemId);
            definition.Index(record => record.DeviceId);
            definition.Index(record => record.DeviceTypeId);
            definition.Index(record => record.PipelineModuleId);
            definition.Index(record => record.ActivityId);
        }

        public Task WriteAsync(string organizationId, string organization, LogRecord record, bool isError)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            if (String.IsNullOrWhiteSpace(organizationId)) throw new ArgumentNullException(nameof(organizationId));

            return _store.InsertAsync(new RuntimeLogActivityRecord
            {
                Id = String.IsNullOrWhiteSpace(record.Id.Value) ? Guid.NewGuid().ToString("N") : record.Id.Value,
                OrganizationId = organizationId,
                Organization = organization,
                CreationDate = record.TimeStamp.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(record.TimeStamp, DateTimeKind.Utc) : record.TimeStamp.ToUniversalTime(),
                IsError = isError,
                LogLevel = record.LogLevel,
                Application = record.Application,
                Environment = record.Environment,
                ExceptionType = record.ExceptionType,
                HostId = record.HostId,
                Version = record.Version,
                PemId = record.PemId,
                InstanceId = record.InstanceId,
                PipelineModuleId = record.PipelineModuleId,
                DeviceTypeId = record.DeviceTypeId,
                DeviceId = record.DeviceId,
                ActivityId = record.ActivityId,
                ErrorCode = record.ErrorCode,
                Area = record.Area,
                Tag = record.Tag,
                Measure = record.Measure,
                MS = record.MS,
                Message = record.Message,
                Details = record.Details,
                StackTrace = record.StackTrace,
                OldState = record.OldState,
                NewState = record.NewState,
                ParametersJson = record.Parameters == null ? null : JsonConvert.SerializeObject(record.Parameters)
            });
        }

        public Task<ListResponse<LogRecord>> QueryErrorsAsync(EntityHeader organization, ListRequest request)
        {
            return QueryInternalAsync(organization, request, true, null);
        }

        public Task<ListResponse<LogRecord>> QueryAsync(EntityHeader organization, RuntimeLogResourceType resourceType, string resourceId, bool isError, ListRequest request)
        {
            if (String.IsNullOrWhiteSpace(resourceId)) throw new ArgumentNullException(nameof(resourceId));

            return QueryInternalAsync(organization, request, isError, query => resourceType switch
            {
                RuntimeLogResourceType.Host => query.Where(record => record.HostId, StorageFilterOperator.Equal, resourceId),
                RuntimeLogResourceType.Instance => query.Where(record => record.InstanceId, StorageFilterOperator.Equal, resourceId),
                RuntimeLogResourceType.Pem => query.Where(record => record.PemId, StorageFilterOperator.Equal, resourceId),
                RuntimeLogResourceType.Device => query.Where(record => record.DeviceId, StorageFilterOperator.Equal, resourceId),
                RuntimeLogResourceType.DeviceType => query.Where(record => record.DeviceTypeId, StorageFilterOperator.Equal, resourceId),
                RuntimeLogResourceType.PipelineModule => query.Where(record => record.PipelineModuleId, StorageFilterOperator.Equal, resourceId),
                RuntimeLogResourceType.Activity => query.Where(record => record.ActivityId, StorageFilterOperator.Equal, resourceId),
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            });
        }

        private async Task<ListResponse<LogRecord>> QueryInternalAsync(
            EntityHeader organization,
            ListRequest request,
            bool isError,
            Func<HistoryQuery<RuntimeLogActivityRecord>, HistoryQuery<RuntimeLogActivityRecord>> applyResourceFilter)
        {
            if (EntityHeader.IsNullOrEmpty(organization)) throw new ArgumentNullException(nameof(organization));
            request ??= ListRequest.Create(1, 100);

            if (!request.TryGetDateRange(out var startUtc, out var endExclusiveUtc, out var dateError))
                return ListResponse<LogRecord>.FromError(dateError);

            var endUtc = endExclusiveUtc.HasValue ? endExclusiveUtc.Value.AddTicks(-1) : DateTime.UtcNow;
            var start = startUtc ?? endUtc.Subtract(DefaultQueryWindow);
            var pageSize = request.PageSize <= 0 ? 100 : Math.Min(request.PageSize, 1000);
            var continuationToken = String.Equals(request.NextPartitionKey, "cassandra", StringComparison.OrdinalIgnoreCase)
                ? request.NextRowKey
                : null;

            var query = new HistoryQuery<RuntimeLogActivityRecord>()
                .Between(start, endUtc)
                .Where(record => record.OrganizationId, StorageFilterOperator.Equal, organization.Id)
                .Where(record => record.IsError, StorageFilterOperator.Equal, isError)
                .WithPage(new StoragePageRequest(pageSize, continuationToken));

            if (applyResourceFilter != null)
                query = applyResourceFilter(query);

            var page = await _store.QueryAsync(query).ConfigureAwait(false);
            var records = page.Items.Select(ToLogRecord).ToList();

            return ListResponse<LogRecord>.Create(
                records,
                request,
                page.HasMoreRecords,
                page.HasMoreRecords ? "cassandra" : null,
                page.ContinuationToken);
        }

        private static LogRecord ToLogRecord(RuntimeLogActivityRecord record)
        {
            Dictionary<string, string> parameters = null;
            if (!String.IsNullOrWhiteSpace(record.ParametersJson))
            {
                try { parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(record.ParametersJson); }
                catch { parameters = null; }
            }

            return new LogRecord
            {
                LogLevel = record.LogLevel,
                Application = record.Application,
                Environment = record.Environment,
                ExceptionType = record.ExceptionType,
                HostId = record.HostId,
                Version = record.Version,
                PemId = record.PemId,
                InstanceId = record.InstanceId,
                PipelineModuleId = record.PipelineModuleId,
                DeviceTypeId = record.DeviceTypeId,
                DeviceId = record.DeviceId,
                ActivityId = record.ActivityId,
                TimeStamp = record.CreationDate,
                ErrorCode = record.ErrorCode,
                Area = record.Area,
                Tag = record.Tag,
                Measure = record.Measure,
                MS = record.MS,
                Message = record.Message,
                Details = record.Details,
                StackTrace = record.StackTrace,
                OldState = record.OldState,
                NewState = record.NewState,
                Parameters = parameters ?? new Dictionary<string, string>()
            };
        }
    }
}
