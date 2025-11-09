// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7f47c89343a573534feeb8c9468322b4926d5f0f1d43d2b77ec23acf5464a618
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
    public interface INotificationLandingPage
    {
        Task<InvokeResult<NotificationLinks>> PreparePage(string raisedNotificationId, string errorId, DeviceNotification notification, bool testMode,
            Device device, OrgLocation location, EntityHeader org, EntityHeader user);
    }
}
