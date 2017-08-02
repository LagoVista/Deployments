using LagoVista.Core.Models;
using System;
using LagoVista.Core;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using LagoVista.Core.Attributes;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum DeploymentActivityTaskTypes
    {
        Deploy,
        Create,
        Start,
        Pause,
        Reset,
        Stop,
        Reload,
        Remove,
        Update,
    }

    public enum DeploymentActivityResourceTypes
    {
        DNS,
        Instance,
        InstanceContainer,
        Server,
        ServerContainer,
        LoadBalancer
    }

    public enum DeploymentActivityStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        PendingRetry,
    }

    public class DeploymentActivity : TableStorageEntity
    {
        public DeploymentActivity(DeploymentActivityResourceTypes resourceType, String resourceId, DeploymentActivityTaskTypes taskType)
        {
            RowKey = DateTime.UtcNow.ToInverseTicksRowKey();
            PartitionKey = resourceId;
            ResourceType = resourceType;
            Type = taskType;
            EnqueueTimeStamp = DateTime.UtcNow.ToJSONString();
            RetryCount = 1;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentActivityTaskTypes Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentActivityResourceTypes ResourceType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentActivityStatus Status { get; set; }

        public void Transition(DeploymentActivityStatus newState, string notes)
        {
            Status = newState;
        }

        public String ResourceId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public string ExternalActivityId { get; set; }

        public String RequestedByUserId { get; set; }
        public String RequestedByUserName { get; set; }

        public String RequestedByOrganizationId { get; set; }
        public String RequestedByOrganizationName { get; set; }

        public int RetryCount { get; set; }

        public int RetryPauseSeconds { get; set; }

        public EntityHeader ToUserEntityHeader()
        {
            return EntityHeader.Create(RequestedByUserId, RequestedByUserName);
        }

        public EntityHeader ToOrgEntityHeader()
        {
            return EntityHeader.Create(RequestedByOrganizationId, RequestedByOrganizationName);
        }

        public string EnqueueTimeStamp { get; set; }
        public string StartTimeStamp { get; set; }
        public string LastAttemptTimeStamp { get; set; }
        public string EndTimeStamp { get; set; }
        public string TimeoutTimeStamp { get; set; }
        public string ErrorMessage { get; set; }
        public double DurationMS { get; set; }

        public DeploymentActivitySummary CreateSummary()
        {
            return new DeploymentActivitySummary()
            {
                Id = RowKey,
                Start = StartTimeStamp,
                ActivityType = Type.ToString(),
                ResourceType = ResourceType.ToString(),
                Status = Status.ToString(),
                DurationMS = Math.Round(DurationMS, 2).ToString(),
                ErrorMessage = ErrorMessage
            };
        }

    }

    public class DeploymentActivitySummary
    {
        public string Id { get; set; }
        [ListColumn(HeaderResource: Resources.DeploymentAdminResources.Names.DeploymentActivity_Start, Ordinal: 1, ResourceType: typeof(Resources.DeploymentAdminResources))]
        public string Start { get; set; }
        [ListColumn( HeaderResource:Resources.DeploymentAdminResources.Names.DeploymentActivity_ActivityType, Ordinal:2, ResourceType:typeof(Resources.DeploymentAdminResources))]
        public string ActivityType { get; set; }
        [ListColumn(HeaderResource: Resources.DeploymentAdminResources.Names.DeploymentActivity_ResourceType, Ordinal: 3, ResourceType: typeof(Resources.DeploymentAdminResources))]
        public string ResourceType { get; set; }
        [ListColumn(HeaderResource: Resources.DeploymentAdminResources.Names.DeploymentActivity_Status, Ordinal: 4, ResourceType: typeof(Resources.DeploymentAdminResources))]
        public string Status { get; set; }
        [ListColumn(HeaderResource: Resources.DeploymentAdminResources.Names.DeploymentActivity_Duration, Ordinal: 5, ResourceType: typeof(Resources.DeploymentAdminResources))]
        public string DurationMS { get; set; }
        [ListColumn(HeaderResource: Resources.DeploymentAdminResources.Names.DeploymentActivity_ErrorMessage, Ordinal: 6, ResourceType: typeof(Resources.DeploymentAdminResources))]
        public string ErrorMessage { get; set; }
    }
}
