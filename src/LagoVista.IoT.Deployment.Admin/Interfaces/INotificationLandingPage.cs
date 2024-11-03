using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.UserAdmin.Models.Orgs;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface INotificationLandingPage
    {
        Task<InvokeResult<NotificationLinks>> PreparePage(string raisedNotificationId, string errorId, DeviceNotification notification, bool testMode,
            Device device, OrgLocation location, EntityHeader org, EntityHeader user);
    }
}
