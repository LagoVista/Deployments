// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 152838948614d9b248745520303774bf1e30d592183d1c40a5949f414a17b2b6
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IEmailSender
    {
        int EmailsSent { get; }
        Task<InvokeResult> PrepareMessage(DeviceNotification notification, bool testMode, Device device, OrgLocation location);
        Task<InvokeResult> SendAsync(string id, DeviceNotification notification, NotificationRecipient recipient, bool allowSilence, NotificationLinks links, EntityHeader org, EntityHeader user);
    }
}
