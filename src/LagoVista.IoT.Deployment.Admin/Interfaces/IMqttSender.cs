using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IMqttSender
    {
        Task<InvokeResult> SendAsync(Mqtt mqtt, Device device, OrgLocation location, EntityHeader org, EntityHeader user);
    }
}
