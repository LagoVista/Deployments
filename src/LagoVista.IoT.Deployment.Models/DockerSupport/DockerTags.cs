using Newtonsoft.Json;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models.DockerSupport
{
    public class DockerImage
    {
        [JsonProperty("size")]
        public long Size { get; set; }
        [JsonProperty("architecture")]
        public string Architecture { get; set; }
        [JsonProperty("variant")]
        public string Variant { get; set; }
        [JsonProperty("features")]
        public string Features { get; set; }
        [JsonProperty("os")]
        public string OS { get; set; }
        [JsonProperty("os_version")]
        public string OSVersion { get; set; }
        [JsonProperty("os_features")]
        public string OSFeatures { get; set; }
    }

    public class DockerTag
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("full_size")]
        public long FullSize { get; set; }
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("repository")]
        public long Repository { get; set; }
        [JsonProperty("creator")]
        public long Creator { get; set; }
        [JsonProperty("last_updater")]
        public long LastUpdater { get; set; }
        [JsonProperty("last_updated")]
        public string LastUpdated { get; set; }
        [JsonProperty("image_id")]
        public string ImageId { get; set; }
        [JsonProperty("v2")]
        public bool V2 { get; set; }


        [JsonProperty("images")]
        public List<DockerImage> Images { get; set; }
    }

    public class DockerTagsResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public List<DockerTag> Tags { get; set; }
    }
}
