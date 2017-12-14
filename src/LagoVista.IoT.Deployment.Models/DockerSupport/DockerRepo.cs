using Newtonsoft.Json;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Admin.Models.DockerSupport
{
    public class DockerRepo
    {
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("repository_type")]
        public string RepositoryType { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("can_edit")]
        public bool CanEdit { get; set; }

        [JsonProperty("star_count")]
        public int StarCount { get; set; }

        [JsonProperty("pull_count")]
        public int PullCount { get; set; }

        [JsonProperty("last_updated")]
        public string LastUpdated { get; set; }

        [JsonProperty("build_on_cloud")]
        public string BuildOnCloud { get; set; }
    }

    public class DockerRepoResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public List<DockerRepo> Repos { get; set; }
    }
}
