// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ea0a4cbaa5b70d0976386c976a0e315f6e2e753a46fec971d63d7fe1b379b562
// IndexVersion: 2
// --- END CODE INDEX META ---
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
