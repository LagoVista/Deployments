// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2bd98bfa404f7cb147e85a81589cf0dfc7ef388976dfff05c1dad044f8dc1f19
// IndexVersion: 2
// --- END CODE INDEX META ---
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
