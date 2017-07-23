using LagoVista.IoT.Deployment.Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IContainerRepo
    {
        Task AddContainerAsync(Container container);
        Task UpdateContainerAsync(Container container);
        Task<Container> GetContainerAsync(string containerId);
        Task<IEnumerable<ContainerSummary>> GetContainersForOrgAsync(string orgId);
    }
}
