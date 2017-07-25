using LagoVista.Core.Models;
using System;
using LagoVista.Core;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public enum DeploymentActivityTaskTypes
    {
        Deploy,
        Stop,
        Start,
        Reset,
        Destroy,
        Update,
    }

    public enum DeploymentActivityResourceTypes
    {
        DNS,
        Server,
        DockerImage,
        LoadBalancer
    }

    public enum DeploymentActivityStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
    }

    public class DeploymentActivity : TableStorageEntity
    {
        public DeploymentActivity(DeploymentActivityResourceTypes resourceType, String resourceId, DeploymentActivityTaskTypes taskType)
        {
            RowKey = DateTime.UtcNow.ToInverseTicksRowKey();
            PartitionKey = resourceId;
            ResourceType = resourceType;
            Type = taskType;
            Start = DateTime.UtcNow.ToJSONString();
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


        public EntityHeader ToUserEntityHeader()
        {
            return EntityHeader.Create(RequestedByUserId, RequestedByUserName);
        }

        public EntityHeader ToOrgEntityHeader()
        {
            return EntityHeader.Create(RequestedByOrganizationId, RequestedByOrganizationName);
        }

        public string Start { get; set; }
        public string End { get; set; }
        public double DurationMS { get; set; }
    }
}
