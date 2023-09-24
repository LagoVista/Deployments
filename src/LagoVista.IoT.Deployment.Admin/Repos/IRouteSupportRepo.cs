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
