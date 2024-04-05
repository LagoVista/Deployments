using LagoVista.Core.Models;
using System;
using LagoVista.Core;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum DeploymentActivityTaskTypes
    {
        DeployHost,
        DeployContainer,        
        Start,
        Pause,
        Stop,
        RestartContainer,
        RestartHost,        
        ReloadSolution,
        UpdateRuntime,
        DestroyHost,
        CreateDNSEntry,
        UpdateDNSEntry,
        DestroyDNSEntry,
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
        Queued,
        Pending,        
        Running,
        Completed,
        Failed,
        TimedOut,
        Scheduled,
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
            Status = DeploymentActivityStatus.Queued;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentActivityTaskTypes Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentActivityResourceTypes ResourceType { get; set; }

        private DeploymentActivityStatus _status;
        [JsonConverter(typeof(StringEnumConverter))]
        public DeploymentActivityStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                LastUpdated = DateTime.UtcNow.ToJSONString();
            }
        }
        

        public string LastUpdated { get; set; }

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

        /// <summary>
        /// How many retries are available, will be decremented after being checked.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// How long to pause between retries
        /// </summary>
        public int RetryPauseSeconds { get; set; }


        /// <summary>
        /// Time Stamp Value for how long this item should "spin" until it's executed.
        /// </summary>
        public string ScheduledFor { get; set; }

        public EntityHeader ToUserEntityHeader()
        {
            return EntityHeader.Create(RequestedByUserId, RequestedByUserName);
        }

        public EntityHeader ToOrgEntityHeader()
        {
            return EntityHeader.Create(RequestedByOrganizationId, RequestedByOrganizationName);
        }

        /// <summary>
        /// When the item was enqueued for processing
        /// </summary>
        public string EnqueueTimeStamp { get; set; }

        /// <summary>
        /// When this item began processing
        /// </summary>
        public string StartTimeStamp { get; set; }

        /// <summary>
        /// Time stamp for when this item was last initiated.
        /// </summary>
        public string LastAttemptTimeStamp { get; set; }

        /// <summary>
        /// When the item either succeeded for failed processing.
        /// </summary>
        public string EndTimeStamp { get; set; }

        /// <summary>
        /// Point at which this item should be considered failed if not completed.
        /// </summary>
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

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeploymentInstanceStatus_Title, DeploymentAdminResources.Names.DeploymentInstanceStatus_Description,
     DeploymentAdminResources.Names.DeploymentInstanceStatus_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-pz-report-4",
     GetListUrl: "/api/deployment/instance/{id}/statushistory")]
    public class DeploymentActivitySummary
    {
        public string Id { get; set; }
        [ListColumn(HeaderResource: DeploymentAdminResources.Names.DeploymentActivity_Start, Ordinal: 1, ResourceType: typeof(DeploymentAdminResources))]
        public string Start { get; set; }
        [ListColumn( HeaderResource:DeploymentAdminResources.Names.DeploymentActivity_ActivityType, Ordinal:2, ResourceType:typeof(DeploymentAdminResources))]
        public string ActivityType { get; set; }
        [ListColumn(HeaderResource: DeploymentAdminResources.Names.DeploymentActivity_ResourceType, Ordinal: 3, ResourceType: typeof(DeploymentAdminResources))]
        public string ResourceType { get; set; }
        [ListColumn(HeaderResource: DeploymentAdminResources.Names.DeploymentActivity_Status, Ordinal: 4, ResourceType: typeof(DeploymentAdminResources))]
        public string Status { get; set; }
        [ListColumn(HeaderResource: DeploymentAdminResources.Names.DeploymentActivity_Duration, Ordinal: 5, ResourceType: typeof(DeploymentAdminResources))]
        public string DurationMS { get; set; }
        [ListColumn(HeaderResource: DeploymentAdminResources.Names.DeploymentActivity_ErrorMessage, Ordinal: 6, ResourceType: typeof(DeploymentAdminResources))]
        public string ErrorMessage { get; set; }
    }
}
