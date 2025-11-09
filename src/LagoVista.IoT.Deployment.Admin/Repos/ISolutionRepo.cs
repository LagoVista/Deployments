// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5f794f22568490320fea0cfc81f281348125555901401d7baba251dc667e1134
// IndexVersion: 2
// --- END CODE INDEX META ---
using System.Collections.Generic;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface ISolutionRepo
    {
        Task AddSolutionAsync(Solution deployment);
        Task<Solution> GetSolutionAsync(string id, bool populateChildren = false);
        Task<ListResponse<SolutionSummary>> GetSolutionsForOrgsAsync(string orgId, ListRequest listRequest);
        Task UpdateSolutionAsync(Solution deployment);
        Task DeleteSolutionAsync(string id);
        Task<bool> QueryKeyInUseAsync(string key, string org);
    }
}
