using LagoVista.IoT.Deployment.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeviceMessageDefinitionRepo 
    {
        Task AddDeviceMessageDefinitionAsync(DeviceMessageDefinition deviceMessageDefinition);
        Task<DeviceMessageDefinition> GetDeviceMessageDefinitionAsync(string id);
        Task<IEnumerable<DeviceMessageDefinitionSummary>> GetDeviceMessageDefinitionsForOrgAsync(string orgId);
        Task UpdateDeviceMessageDefinitionAsync(DeviceMessageDefinition deviceMessageDefinition);
        Task DeleteDeviceMessageDefinitionAsync(string id);
        Task<bool> QueryKeyInUseAsync(string key, string orgId);
    }
}
