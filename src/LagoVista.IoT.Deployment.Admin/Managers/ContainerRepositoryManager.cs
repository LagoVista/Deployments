// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 324c54025bf28583adc6f99bb3c9cb5949a067bc73882b818d35bd6016a78b80
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class ContainerRepositoryManager : ManagerBase, IContainerRepositoryManager
    {
        IContainerRepositoryRepo _repo;
        ISecureStorage _secureStorage;
        IDockerRegisteryServices _dockerRegistryServices;

        public ContainerRepositoryManager(IContainerRepositoryRepo repo,  ISecureStorage secureStorage, IAdminLogger logger, IDockerRegisteryServices dockerRegistryServices,
                IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) 
            : base(logger, appConfig, dependencyManager, security)
        {
            _repo = repo;
            _secureStorage = secureStorage;
            _dockerRegistryServices = dockerRegistryServices;
        }

        public async Task<InvokeResult> AddContainerRepoAsync(ContainerRepository containerRepo, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(containerRepo, Actions.Create);
            await AuthorizeAsync(containerRepo, AuthorizeResult.AuthorizeActions.Create, user, org);

            if(String.IsNullOrEmpty(containerRepo.Password))
            {
                return InvokeResult.FromErrors(new ErrorMessage("Password is required."));
            }

            var addSecretResult = await _secureStorage.AddSecretAsync(org, containerRepo.Password);
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

            if(!String.IsNullOrEmpty(containerRepo.Password))
            {
                await _secureStorage.RemoveSecretAsync(org, containerRepo.SecurePasswordId);

                var addSecretResult = await _secureStorage.AddSecretAsync(org, containerRepo.Password);
                if (!addSecretResult.Successful) return addSecretResult.ToInvokeResult();
                containerRepo.SecurePasswordId = addSecretResult.Result;
                containerRepo.Password = null;
            }

            await _repo.UpdateContainerRepoAsync(containerRepo);
            return InvokeResult.Success;
        }

        public async Task<ContainerRepository> GetContainerRepoAsync(string id, EntityHeader org, EntityHeader user)
        {
            var containerRepo = await _repo.GetContainerRepoAsync(id);
            await AuthorizeAsync(containerRepo, AuthorizeResult.AuthorizeActions.Read, user, org);
            return containerRepo;
        }

        public async Task<IEnumerable<ContainerRepositorySummary>> GetContainerReposForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(TaggedContainer), Actions.Read);
            return await _repo.GetContainerReposForOrgAsync(orgId);
        }

        public async Task<IEnumerable<DockerTag>> GetTagsFromRemoteRegistryAsync(string containerId, EntityHeader org, EntityHeader user)
        {
            var containerRepo = await _repo.GetContainerRepoAsync(containerId);
            await AuthorizeAsync(containerRepo, AuthorizeResult.AuthorizeActions.Read, user, org);

            var pwdResult = await _secureStorage.GetSecretAsync(org, containerRepo.SecurePasswordId, user);
            if(!pwdResult.Successful)
            {
                throw new UnauthorizedAccessException("Could not retrieve password from secure password, please check your password.");
            }

            var tokenResult = await _dockerRegistryServices.GetTokenAsync(containerRepo.UserName, pwdResult.Result);
            if (!tokenResult.Successful)
            {
                throw new UnauthorizedAccessException("Could not authorize access to repostiory, please check your password.");
            }

            var tagsResult = await _dockerRegistryServices.GetTagsAsync(containerRepo.Namespace, containerRepo.RepositoryName, tokenResult.Result);
            return tagsResult.Result.Tags;
        }

        public Task<ContainerRepository> GetDefaultForRuntimeRepoAsync(EntityHeader org, EntityHeader user)
        {
            return _repo.GetDefaultForRuntimeAsync();
        }
    }
}