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
