using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IDeploymentInstanceStatusRepo
    {
        Task AddDeploymentInstanceStatusAsync(DeploymentInstanceStatus instanceStatus);
        Task<ListResponse<DeploymentInstanceStatus>> GetStatusHistoryForInstanceAsync(string instanceId, ListRequest listRequest);
    }
}
