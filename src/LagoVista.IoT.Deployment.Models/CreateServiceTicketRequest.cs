// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 10febb5e8f3a9ce06e14f52c6c0f2242642c8c8315f686cb2b7eba48d4728fd4
// IndexVersion: 0
// --- END CODE INDEX META ---
namespace LagoVista.IoT.Deployment.Models
{
    public class CreateServiceTicketRequest
    {
        public string BoardId { get; set; }
        public string TemplateId { get; set; }
        public string TemplateKey { get; set; }
        public string RepoId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceId { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public bool DontCreateIfOpenForDevice { get; set; }
    }
}
