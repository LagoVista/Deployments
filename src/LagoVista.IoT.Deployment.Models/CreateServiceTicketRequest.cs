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
