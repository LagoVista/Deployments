// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f590df366d55988d93613aec81a637083b4b284ab29a9a5b6af16179b412a8b7
// IndexVersion: 0
// --- END CODE INDEX META ---
using Newtonsoft.Json;
using System;
using LagoVista.Core;
using LagoVista.IoT.Logging.Models;
using LagoVista.Core.Attributes;
using LagoVista.IoT.Deployment.Models.Resources;

namespace LagoVista.IoT.Deployment.Admin.Models
{

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Telemetry_Title, DeploymentAdminResources.Names.Telemetry_Help,
         DeploymentAdminResources.Names.Telemetry_Help, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-pz-report-4",
         GetListUrl: "/api/telemetry/events/instance/{id}")]
    public class TelemetryReportData
    {
        [JsonProperty("id")]
        public String Id { get; set; }

        [JsonProperty("timeStamp")]
        public String TimeStamp { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("itemId")]
        public String ItemId { get; set; }

        [JsonProperty("itemType")]
        public String ItemType { get; set; }


        [JsonProperty("tag")]
        public String Tag { get; set; }

        [JsonProperty("instanceId")]
        public String InstanceId { get; set; }

        [JsonProperty("hostId")]
        public String HostId { get; set; }

        [JsonProperty("activityId")]
        public String ActivityId { get; set; }

        [JsonProperty("pemId")]
        public String PemId { get; set; }

        [JsonProperty("pipelineModuleId")]
        public String PipelineModuleId { get; set; }

        [JsonProperty("deviceId")]
        public String DeviceId { get; set; }

        [JsonProperty("deviceTypeId")]
        public String DeviceTypeId { get; set; }

        [JsonProperty("level")]
        public String Level { get; set; }


        public static TelemetryReportData FromLogRecord(LogRecord logRecord, string recordType)
        {
            var trd = new TelemetryReportData()
            {
                Id = logRecord.Id,
                TimeStamp = logRecord.TimeStamp.ToJSONString(),
                Message = logRecord.Message,
                Level = logRecord.LogLevel,
                ItemType = recordType,
                DeviceId = logRecord.DeviceId,
                DeviceTypeId = logRecord.DeviceTypeId,
                HostId = logRecord.HostId,
                InstanceId = logRecord.InstanceId,
                ActivityId = logRecord.ActivityId,
                PemId = logRecord.PemId,
                NewState = logRecord.NewState,
                OldState = logRecord.OldState,
                PipelineModuleId = logRecord.PipelineModuleId,
                Tag = logRecord.Tag,
                Details = logRecord.Details,
                Version = logRecord.Version
            };

            return trd;
        }

        [JsonProperty("setting")]
        public String Setting { get; set; }

        [JsonProperty("version")]
        public String Version { get; set; }

        [JsonProperty("oldState")]
        public String OldState { get; set; }

        [JsonProperty("newState")]
        public String NewState { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }


        /* maybe add these back in at somepoint 
         * 
         
        public String Name { get; set; }

        [JsonProperty("orgId")]
        public String OrgId { get; set; }

        [JsonProperty("orgName")]
        public String OrgName { get; set; }

        [JsonProperty("userId")]
        public String UserId { get; set; }

        [JsonProperty("email")]
        public String Email { get; set; }

        [JsonProperty("user")]
        public String User { get; set; }

    */

    }
}
