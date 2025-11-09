// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 201802afbac6fa0a75c0691888a1597e3cf635efc2a32755d063be5967badb09
// IndexVersion: 2
// --- END CODE INDEX META ---
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
