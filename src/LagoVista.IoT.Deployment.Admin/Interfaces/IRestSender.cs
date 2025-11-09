// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7eadf986780b5a3714a842c59c61e31e4f00aa73a5b9ad4fead9655cff139ad5
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IRestSender
    {
        Task<InvokeResult> SendAsync(Rest rest, Device device, OrgLocation location, EntityHeader org, EntityHeader user);
    }
}
