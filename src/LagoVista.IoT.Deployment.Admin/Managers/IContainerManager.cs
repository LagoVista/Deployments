using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public interface IContainerManager
    {
        Task<InvokeResult> AddContainerAsync(Container container, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateContainerAsync(Container container, EntityHeader org, EntityHeader user);
        Task<Container> GetContainerAsync(string id, EntityHeader org, EntityHeader user);
        Task<IEnumerable<ContainerSummary>> GetContainersForOrgAsync(string orgId, EntityHeader user);
    }
}
