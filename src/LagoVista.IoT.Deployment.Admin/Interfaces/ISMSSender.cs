using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface ISMSSender
    {
        int TextMessagesSent { get; }
         Task<InvokeResult> PrepareMessage(DeviceNotification notification, Device device, OrgLocation location);
        Task<InvokeResult> SendAsync(NotificationRecipient recipient, NotificationLinks links, EntityHeader org, EntityHeader user);
    }
}
