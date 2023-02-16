using LagoVista.Core.Models;

namespace LagoVista.IoT.Deployment.Models
{
    public class ServerConnectionProfile
    {
        public string ListenerId { get; set; }

        public EntityHeader<Pipeline.Admin.Models.ListenerTypes> ListenerType { get; set; }

        public string HostName { get; set; }

        public int Port { get; set; }

        public bool Anonymous { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
