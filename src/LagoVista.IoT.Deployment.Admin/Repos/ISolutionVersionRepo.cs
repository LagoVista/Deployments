using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface ISolutionVersionRepo
    {
        Task<SolutionVersion> PublishSolutionVersionAsync(SolutionVersion instanceVersion, Solution instance);

        Task<IEnumerable<SolutionVersion>> GetSolutionVersionsAsync(string solutionId);

        Task UpdateSolutionVersionStatusAsync(string solutionId, string versionId, string newStatus);

        Task<InvokeResult<Solution>> GetSolutionVersionAsync(string solutionId, string versionId);
    }
}
