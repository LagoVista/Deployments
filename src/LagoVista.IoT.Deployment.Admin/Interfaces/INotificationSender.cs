// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7f24492fe2ae8cba218c251eacf9fcb15bbc9f80abf32d642762ca9386ca847b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface INotificationSender
    {
        Task<InvokeResult<string>> RaiseNotificationAsync(RaisedDeviceNotification raisedNotification, EntityHeader orgEntityHeader, EntityHeader userEntityHeader);

        Task<InvokeResult> SendDeviceOnlineNotificationAsync(Device device, string lastContact, bool testMode, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SendDeviceOfflineNotificationAsync(Device device, string lastContact, bool testMode, EntityHeader org, EntityHeader user);
    }
}
