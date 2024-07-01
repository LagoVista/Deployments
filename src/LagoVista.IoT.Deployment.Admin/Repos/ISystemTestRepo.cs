using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface ISystemTestRepo
    {
        Task AddSystemTestAsync(SystemTest errorCode);
        Task DeleteSystemTestAsync(string id);
        Task<SystemTest> GetSystemTestAsync(string id);
        Task<SystemTest> GetSystemTestByKeyAsync(string key, string orgId);
        Task<ListResponse<SystemTestSummary>> GetSystemTestsForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateSystemTestAsync(SystemTest errorCode);
    }
}
