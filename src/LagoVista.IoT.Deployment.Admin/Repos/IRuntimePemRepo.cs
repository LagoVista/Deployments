using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IRuntimePemRepo
    {
        Task UpsertAsync(RuntimePemStorageRecord record);
        Task<RuntimePemStorageRecord> GetByDeviceMessageAsync(string organizationId, string deviceId, string messageId);
    }
}
