using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class TelemetryReportData
    {
        public String Name { get; set; }

        public String TimeStamp { get; set; }

        [JsonProperty("tag")]
        public String Tag { get; set; }

        [JsonProperty("instanceId")]
        public String InstanceId { get; set; }

        [JsonProperty("hostId")]
        public String HostId { get; set; }

        [JsonProperty("pipelineModuleId")]
        public String PipelineModuleId { get; set; }

        [JsonProperty("deviceId")]
        public String DeviceId { get; set; }

        [JsonProperty("deviceTypeId")]
        public String DeviceTypeId { get; set; }

        [JsonProperty("level")]
        public String Level { get; set; }

        [JsonProperty("setting")]
        public String Setting { get; set; }

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

        [JsonProperty("oldState")]
        public String OldState { get; set; }

        [JsonProperty("newState")]
        public String NewState { get; set; }
    }
}
