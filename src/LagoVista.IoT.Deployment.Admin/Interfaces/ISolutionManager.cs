using LagoVista.Core.Models;
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

        Task<IEnumerable<SolutionSummary>> GetSolutionsForOrgsAsync(string id, EntityHeader user);
        Task<InvokeResult> UpdateSolutionsAsync(Solution deployment, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteSolutionAsync(string id, EntityHeader org, EntityHeader user);

        Task<bool> QueryKeyInUse(string key, EntityHeader org);

        ValidationResult ValidateSolution(Solution solution);
        Task<ValidationResult> ValidateSolutionAsync(string id, EntityHeader org, EntityHeader user);
    }
}
