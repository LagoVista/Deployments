using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Globalization;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    // Immutable projection of a terminal deployment activity. The mutable
    // DeploymentActivity model remains the shared execution/API contract.
    public abstract class DeploymentActivityHistoryRecord : IActivityRecord
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public DateTime CreationDate { get; set; }
        public string ResourceId { get; set; }
        public string Start { get; set; }
        public string ActivityType { get; set; }
        public string ResourceType { get; set; }
        public string Status { get; set; }
        public double DurationMS { get; set; }
        public string ErrorMessage { get; set; }

        public DeploymentActivitySummary CreateSummary() => new DeploymentActivitySummary
        {
            Id = Id,
            Start = Start,
            ActivityType = ActivityType,
            ResourceType = ResourceType,
            Status = Status,
            DurationMS = Math.Round(DurationMS, 2).ToString(CultureInfo.InvariantCulture),
            ErrorMessage = ErrorMessage
        };

        public static void ConfigureStorage<TRecord>(StorageDefinition<TRecord> definition)
            where TRecord : DeploymentActivityHistoryRecord
        {
            // Existing history lookup takes a resource ID without an organization
            // or date range. Keep that lookup a bounded Cassandra partition read.
            definition.PartitionBy(record => record.ResourceId);
        }

        protected static void CopyFrom(DeploymentActivity activity, DeploymentActivityHistoryRecord result)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (String.IsNullOrWhiteSpace(activity.PartitionKey)) throw new ArgumentException("Deployment activity has no resource ID.", nameof(activity));
            if (String.IsNullOrWhiteSpace(activity.RowKey)) throw new ArgumentException("Deployment activity has no activity ID.", nameof(activity));

            result.Id = activity.RowKey;
            result.ResourceId = activity.PartitionKey;
            result.OrganizationId = String.IsNullOrWhiteSpace(activity.RequestedByOrganizationId)
                ? "anonymous"
                : activity.RequestedByOrganizationId;
            result.Organization = String.IsNullOrWhiteSpace(activity.RequestedByOrganizationName)
                ? "Anonymous"
                : activity.RequestedByOrganizationName;
            result.CreationDate = ParseTimestamp(activity.EndTimeStamp);
            result.Start = activity.StartTimeStamp;
            result.ActivityType = activity.Type.ToString();
            result.ResourceType = activity.ResourceType.ToString();
            result.Status = activity.Status.ToString();
            result.DurationMS = activity.DurationMS;
            result.ErrorMessage = activity.ErrorMessage;
        }

        private static DateTime ParseTimestamp(string value)
        {
            if (!String.IsNullOrWhiteSpace(value) &&
                DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            {
                return parsed.Kind == DateTimeKind.Utc
                    ? parsed
                    : parsed.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                        : parsed.ToUniversalTime();
            }

            return DateTime.UtcNow;
        }
    }

    public sealed class CompletedDeploymentActivityRecord : DeploymentActivityHistoryRecord
    {
        public static CompletedDeploymentActivityRecord From(DeploymentActivity activity)
        {
            var record = new CompletedDeploymentActivityRecord();
            CopyFrom(activity, record);
            return record;
        }

        public static void ConfigureStorage(StorageDefinition<CompletedDeploymentActivityRecord> definition)
            => ConfigureStorage<CompletedDeploymentActivityRecord>(definition);
    }

    public sealed class FailedDeploymentActivityRecord : DeploymentActivityHistoryRecord
    {
        public static FailedDeploymentActivityRecord From(DeploymentActivity activity)
        {
            var record = new FailedDeploymentActivityRecord();
            CopyFrom(activity, record);
            return record;
        }

        public static void ConfigureStorage(StorageDefinition<FailedDeploymentActivityRecord> definition)
            => ConfigureStorage<FailedDeploymentActivityRecord>(definition);
    }
}
