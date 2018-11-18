using LagoVista.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class DeviceRepoToken
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "expires")]
        public string Expires { get; set; }
        [JsonProperty(PropertyName = "deviceRepo")]
        public EntityHeader DeviceRepo { get; set; }
    }
}
