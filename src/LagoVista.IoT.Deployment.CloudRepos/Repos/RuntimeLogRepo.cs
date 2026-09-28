using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Models;
using Newtonsoft.Json;
using System;
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
    }
}
