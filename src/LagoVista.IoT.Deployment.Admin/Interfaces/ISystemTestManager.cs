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
        Task<InvokeResult<SystemTestExecution>> CompleteStepAsync(string testExecutionId, string stepResultId, TestStepUpdate update, EntityHeader org, EntityHeader user);
        Task<InvokeResult<SystemTestExecution>> StartTestAsync(string systemTestId, EntityHeader org, EntityHeader user);
        Task<ListResponse<SystemTestExecutionSummary>> GetTestResultsAsync(ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<InvokeResult<SystemTestExecution>> GetTestResultAsync(string systemTestExecutionId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<SystemTestExecution>> AbortTestAsync(string systemTestExecutionId, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteExecutionAsync(string id, EntityHeader org, EntityHeader user);
    }
}
