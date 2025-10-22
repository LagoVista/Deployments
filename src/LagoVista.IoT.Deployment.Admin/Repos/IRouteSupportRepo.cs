// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 34ae4b748f936a12d78158bc8db93e9fdf9fdfaf849f16093789831fcb2b6462
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IRouteSupportRepo
    {
        Task SetDefaultPipelineModulesAsync(EntityHeader org, Route route); 
    }
}
