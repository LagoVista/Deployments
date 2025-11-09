// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a29aec2610926574673bd8a8748b7404b7e93d36395e1b69820b9945adfa6681
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface ISolutionManager
    {
        Task<InvokeResult> AddSolutionsAsync(Solution solution, EntityHeader org, EntityHeader user);
        Task<Solution> GetSolutionAsync(string id, EntityHeader org, EntityHeader user);

        Task<InvokeResult<Solution>> LoadFullSolutionAsync(string id, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);

        Task<ListResponse<SolutionSummary>> GetSolutionsForOrgsAsync(string id, ListRequest listRequest, EntityHeader user);
        Task<InvokeResult> UpdateSolutionsAsync(Solution deployment, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteSolutionAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> PublishSolutionAsync(SolutionVersion solutionVersion, EntityHeader org, EntityHeader user);

        Task<IEnumerable<SolutionVersion>> GetVersionsForSolutionAsync(string solutionId, EntityHeader org, EntityHeader user);
        Task<bool> QueryKeyInUse(string key, EntityHeader org);

        ValidationResult ValidateSolution(Solution solution);
        Task<ValidationResult> ValidateSolutionAsync(string id, EntityHeader org, EntityHeader user);
    }
}
