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
