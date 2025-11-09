// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8c7c110a28070855153480b5ff508471f9c91e77dc7e44477279a9e1968da397
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface ISystemTestExecutionRepo
    {
        Task<ListResponse<SystemTestExecutionSummary>> GetSystemTestExecutionsAsync(string orgId, ListRequest listRequest);
        Task AddSystemTestExecutionAsync(SystemTestExecution testExecution);
        Task<SystemTestExecution> GetSystemTestExecutionAsync(string id);
        Task<ListResponse<SystemTestExecutionSummary>> GetSystemTestExecutionsForCustomerAsync(string customerId, string orgId, ListRequest listRequest);
        Task UpdateSystemTestExecutionAsync(SystemTestExecution testExecution);

        Task DeleteTestExecutionAsync(string id);

    }
}
