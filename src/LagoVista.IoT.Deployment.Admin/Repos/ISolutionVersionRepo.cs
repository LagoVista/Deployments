// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1006e84527478fd6d62eefcd62feb2f1b61f4bb4182241b2662e4667973b1032
// IndexVersion: 0
// --- END CODE INDEX META ---
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
