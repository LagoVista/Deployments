using LagoVista.Core.Managers;
using System.Collections.Generic;
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class ContainerManager : ManagerBase, IContainerManager
    {
        IContainerRepo _containerRepo;

        public ContainerManager(IContainerRepo containerRepo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : base(logger, appConfig, dependencyManager, security)
        {
            _containerRepo = containerRepo;
        }

        public async Task<InvokeResult> AddContainerAsync(Container container, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(container, Actions.Create);
            await AuthorizeAsync(container, AuthorizeResult.AuthorizeActions.Create, user, org);
            await _containerRepo.AddContainerAsync(container);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateContainerAsync(Container container, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(container, Actions.Update);
            await AuthorizeAsync(container, AuthorizeResult.AuthorizeActions.Update, user, org);
            await _containerRepo.UpdateContainerAsync(container);
            return InvokeResult.Success;
        }

        public async Task<Container> GetContainerAsync(string id, EntityHeader org, EntityHeader user)
        {
            var container = await _containerRepo.GetContainerAsync(id);
            await AuthorizeAsync(container, AuthorizeResult.AuthorizeActions.Update, user, org);
            return container;
        }

        public async Task<IEnumerable<ContainerSummary>> GetContainersForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Container), Actions.Read);
            return await _containerRepo.GetContainersForOrgAsync(orgId);
        }
    }
}
