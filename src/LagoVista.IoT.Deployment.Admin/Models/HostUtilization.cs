using LagoVista.Core.Models;


namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class HostUtilization : TableStorageEntity
    {
        public string DateStamp { get; set; }
        public double CPU { get; set; }
        public double Network { get; set; }
        public double NetworkPercent { get; set; }
        public double Memory { get; set; }
        public double MemoryPercent { get; set; }
    }
}
