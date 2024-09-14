using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface INotificationSender
    {
        Task<InvokeResult> RaiseNotificationAsync(RaisedDeviceNotification raisedNotification, EntityHeader orgEntityHeader, EntityHeader userEntityHeader);

    }
}
