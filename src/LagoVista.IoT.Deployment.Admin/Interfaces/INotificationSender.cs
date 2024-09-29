using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface INotificationSender
    {
        Task<InvokeResult> RaiseNotificationAsync(RaisedDeviceNotification raisedNotification, EntityHeader orgEntityHeader, EntityHeader userEntityHeader);

        Task<InvokeResult> SendDeviceOnlineNotificationAsync(Device device, long secondsOffline, bool testMode, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SendDeviceOfflineNotificationAsync(Device device, string lastContact, bool testMode, EntityHeader org, EntityHeader user);
    }
}
