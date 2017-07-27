using Newtonsoft.Json;

namespace LagoVista.IoT.Deployment.Admin.Models.DockerSupport
{
    public class TokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
