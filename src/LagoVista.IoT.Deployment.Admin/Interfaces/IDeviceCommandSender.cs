// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d5579fb9784eb206e1f7cd107d9a148fe8f27aa4c7a54634fd21dfa44d560196
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IDeviceCommandSender
    {
        Task<InvokeResult> PrepareMessage(DeviceNotification notification, bool testMode, Device device, OrgLocation location);
        Task<InvokeResult> SendAsync(string instanceId, string id, EntityHeader org, EntityHeader user);
    }
}
