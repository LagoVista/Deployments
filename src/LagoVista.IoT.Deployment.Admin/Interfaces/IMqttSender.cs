// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: dc359394c33e08b770a7975ffceaa6143bd4590b92ffc749e1f6c98ae175ea6a
// IndexVersion: 2
// --- END CODE INDEX META ---
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
