using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    // This is durable mutable execution state, not an activity history record.
    // A single control-plane partition supports MCP's global dispatch queries;
    // terminal activities are removed and written to separate history tables.
    public sealed class ActiveDeploymentActivityRecord : IConditionalOperationalDataRecord
    {
        public const string QueueScope = "deployment-activity-queue";

        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public long Version { get; set; }
        public string ResourceId { get; set; }
        public string Status { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public string PayloadJson { get; set; }

        public static void ConfigureStorage(StorageDefinition<ActiveDeploymentActivityRecord> definition)
        {
            definition.Index(record => record.Status);
            definition.Index(record => record.ResourceId);
            definition.Index(record => record.ScheduledFor);
        }

        public static ActiveDeploymentActivityRecord From(DeploymentActivity activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (String.IsNullOrWhiteSpace(activity.RowKey)) throw new ArgumentException("Deployment activity is missing RowKey.", nameof(activity));
            if (String.IsNullOrWhiteSpace(activity.PartitionKey)) throw new ArgumentException("Deployment activity is missing PartitionKey.", nameof(activity));

            return new ActiveDeploymentActivityRecord
            {
                Id = activity.RowKey,
                OrganizationId = QueueScope,
                ResourceId = activity.PartitionKey,
                Version = activity.StorageVersion,
                CreationDate = ParseTimestamp(activity.EnqueueTimeStamp, "EnqueueTimeStamp") ?? DateTime.UtcNow,
                Status = activity.Status.ToString(),
                ScheduledFor = ParseTimestamp(activity.ScheduledFor, "ScheduledFor"),
                PayloadJson = JsonConvert.SerializeObject(activity)
            };
        }

        public DeploymentActivity ToActivity()
        {
            if (String.IsNullOrWhiteSpace(PayloadJson))
                throw new InvalidOperationException($"Deployment activity {Id} has no serialized payload.");

            var activity = JsonConvert.DeserializeObject<DeploymentActivity>(PayloadJson);
            if (activity == null)
                throw new InvalidOperationException($"Failed to deserialize deployment activity {Id}.");

            if (!String.Equals(activity.RowKey, Id, StringComparison.Ordinal) ||
                !String.Equals(activity.PartitionKey, ResourceId, StringComparison.Ordinal))
                throw new InvalidOperationException($"Deployment activity identity mismatch for {Id} / {ResourceId}.");

            activity.StorageVersion = Version;
            return activity;
        }

        private static DateTime? ParseTimestamp(string value, string field)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
                throw new ArgumentException($"Invalid deployment activity timestamp {field}: '{value}'.");

            return parsed.Kind == DateTimeKind.Utc
                ? parsed
                : parsed.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                    : parsed.ToUniversalTime();
        }
    }
}
