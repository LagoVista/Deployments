using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IRemoteConfigurationManager
    {
        Task<InvokeResult> SendPropertyAsync(string deviceRepoId, string deviceUniqueId, int propertyIndex, EntityHeader org, EntityHeader user);
        Task<InvokeResult> SendAllPropertiesAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> QueryRemoteConfigurationAsync (string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RestartDeviceAsync(string deviceRepoId, string deviceUniqueId, EntityHeader org, EntityHeader user);
    }

    public interface IRemotePropertyNamanager
    {
        Task<InvokeResult> SendPropertyAsync(string deviceUniqueId, int propertyIndex);
        Task<InvokeResult> SendAllPropertiesAsync(string deviceUniqueId);
        Task<InvokeResult> QueryRemoteConfigurationAsync(string deviceUniqueId);
        Task<InvokeResult> RestartDeviceAsync(string deviceUniqueId);
    }
}
