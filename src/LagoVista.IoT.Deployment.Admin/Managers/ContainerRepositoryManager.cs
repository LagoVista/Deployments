using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class ContainerRepositoryManager : ManagerBase
    {
        IContainerRepositoryRepo _repo;
        ISecureStorage _secureStorage;

        public ContainerRepositoryManager(IContainerRepositoryRepo repo,  ISecureStorage secureStorage, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : base(logger, appConfig, dependencyManager, security)
        {
            _repo = repo;
            _secureStorage = secureStorage;
        }

        public async Task<InvokeResult> AddContainerRepoAsync(ContainerRepository containerRepo, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(containerRepo, Actions.Create);
            await AuthorizeAsync(containerRepo, AuthorizeResult.AuthorizeActions.Create, user, org);
            var addSecretResult = await _secureStorage.AddSecretAsync(containerRepo.Password);
            if (!addSecretResult.Successful) return addSecretResult.ToInvokeResult();
            containerRepo.SecurePasswordId = addSecretResult.Result;
            containerRepo.Password = null;
            await _repo.AddContainerRepoAsync(containerRepo);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateContainerRepoAsync(ContainerRepository containerRepo, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(containerRepo, Actions.Update);
            await AuthorizeAsync(containerRepo, AuthorizeResult.AuthorizeActions.Update, user, org);
            await _repo.UpdateContainerRepoAsync(containerRepo);
            return InvokeResult.Success;
        }

        public async Task<ContainerRepository> GetContainerRepoAsync(string id, EntityHeader org, EntityHeader user)
        {
            var containerRepo = await _repo.GetContainerRepoAsync(id);
            await AuthorizeAsync(containerRepo, AuthorizeResult.AuthorizeActions.Update, user, org);
            return containerRepo;
        }

        public async Task<IEnumerable<ContainerRepositorySummary>> GetContainerReposForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Container), Actions.Read);
            return await _repo.GetContainerReposForOrgAsync(orgId);
        }
    }
}
