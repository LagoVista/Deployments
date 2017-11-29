using Newtonsoft.Json;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Text;
using LagoVista.IoT.Logging.Models;

namespace LagoVista.IoT.Deployment.Admin.Models
{
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


        internal static TelemetryReportData FromLogRecord(LogRecord logRecord, string recordType)
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
