using LagoVista.CloudStorage.Storage;
using LagoVista.CloudStorage.Storage.StorageProviders.Cassandra;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public sealed class RuntimePemOperationalRecord : IOperationalDataRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string DeviceId { get; set; }
        public string MessageId { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }
        public string ErrorReason { get; set; }
        public string MessageType { get; set; }
        public string CreatedTimeStamp { get; set; }
        public double TotalProcessingMS { get; set; }
        public string Json { get; set; }
        public string TextPayload { get; set; }
        public string Values { get; set; }
        public string OutgoingMessages { get; set; }
        public string ResponseMessage { get; set; }
        public string Log { get; set; }
        public string Instructions { get; set; }
        public string Device { get; set; }
        public string RuntimeVersion { get; set; }
        public string SolutionVersion { get; set; }
        public bool IsFailure { get; set; }
    }

    public sealed class RuntimePemRepo : IRuntimePemRepo
    {
        private readonly IOperationalDataStore<RuntimePemOperationalRecord> _store;

        public RuntimePemRepo(IOperationalDataStore<RuntimePemOperationalRecord> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        internal static void ConfigureStorage(StorageDefinition<RuntimePemOperationalRecord> definition)
        {
            definition.Index(record => record.DeviceId);
            definition.Index(record => record.MessageId);
            definition.Index(record => record.IsFailure);
            definition.RetainFor(TimeSpan.FromDays(30));
        }

        public Task UpsertAsync(RuntimePemStorageRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            return _store.UpsertAsync(new RuntimePemOperationalRecord
            {
                Id = record.Id,
                OrganizationId = record.OrganizationId,
                DeviceId = record.DeviceId,
                MessageId = record.MessageId,
                Topic = record.Topic,
                Status = record.Status,
                ErrorReason = record.ErrorReason,
                MessageType = record.MessageType,
                CreatedTimeStamp = record.CreatedTimeStamp,
                TotalProcessingMS = record.TotalProcessingMS,
                Json = record.Json,
                TextPayload = record.TextPayload,
                Values = record.Values,
                OutgoingMessages = record.OutgoingMessages,
                ResponseMessage = record.ResponseMessage,
                Log = record.Log,
                Instructions = record.Instructions,
                Device = record.Device,
                RuntimeVersion = record.RuntimeVersion,
                SolutionVersion = record.SolutionVersion,
                IsFailure = record.IsFailure
            });
        }

        public async Task<RuntimePemStorageRecord> GetByDeviceMessageAsync(string organizationId, string deviceId, string messageId)
        {
            var query = new StorageQuery<RuntimePemOperationalRecord>()
                .Where(record => record.OrganizationId, StorageFilterOperator.Equal, organizationId)
                .Where(record => record.DeviceId, StorageFilterOperator.Equal, deviceId)
                .Where(record => record.MessageId, StorageFilterOperator.Equal, messageId)
                .WithPage(new StoragePageRequest(10));

            var page = await _store.QueryAsync(query).ConfigureAwait(false);
            var record = page.Items.OrderByDescending(item => item.LastUpdatedDate).FirstOrDefault();
            if (record == null) return null;

            return new RuntimePemStorageRecord
            {
                Id = record.Id,
                OrganizationId = record.OrganizationId,
                DeviceId = record.DeviceId,
                MessageId = record.MessageId,
                Topic = record.Topic,
                Status = record.Status,
                ErrorReason = record.ErrorReason,
                MessageType = record.MessageType,
                CreatedTimeStamp = record.CreatedTimeStamp,
                TotalProcessingMS = record.TotalProcessingMS,
                Json = record.Json,
                TextPayload = record.TextPayload,
                Values = record.Values,
                OutgoingMessages = record.OutgoingMessages,
                ResponseMessage = record.ResponseMessage,
                Log = record.Log,
                Instructions = record.Instructions,
                Device = record.Device,
                RuntimeVersion = record.RuntimeVersion,
                SolutionVersion = record.SolutionVersion,
                IsFailure = record.IsFailure
            };
        }
    }
}
