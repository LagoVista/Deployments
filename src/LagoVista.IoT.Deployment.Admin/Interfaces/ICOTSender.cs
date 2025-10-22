// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4238c1ad29ff5a0b7e5d6679f940e608377d306761596f7a835e1b9b255fa437
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface  ICOTSender
    {
        Task<InvokeResult> SendAsync(CursorOnTarget cot, Device device, OrgLocation location, EntityHeader org, EntityHeader user);
    }
}
