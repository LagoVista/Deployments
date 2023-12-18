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
