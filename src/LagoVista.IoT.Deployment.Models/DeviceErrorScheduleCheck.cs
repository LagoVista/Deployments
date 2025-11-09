// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3707ea4a6d069597e10645c8e3e161f74acb9278922a86d60b5a4920671406a7
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;

namespace LagoVista.IoT.Deployment.Models
{
    public class DeviceErrorScheduleCheck
    {
        public DeviceException DeviceException { get; set; }
        public string DueTimeStamp { get; set; }
        public EntityHeader Org { get; set; }
        public EntityHeader User { get; set; }
    }
}
