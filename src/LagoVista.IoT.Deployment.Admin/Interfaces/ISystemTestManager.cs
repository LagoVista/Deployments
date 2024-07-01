using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface ISystemTestManager
    {
        Task<InvokeResult> AddSystemTestAsync(SystemTest systemTest, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteSystemTestAsync(string id, EntityHeader org, EntityHeader user);
        Task<SystemTest> GetSystemTestAsync(string id, EntityHeader org, EntityHeader user);
        Task<SystemTest> GetSystemTestByKeyAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<SystemTestSummary>> GetSystemTestsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateSystemTestAsync(SystemTest systemTest, EntityHeader org, EntityHeader user);
    }
}
