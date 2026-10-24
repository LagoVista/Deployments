// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 324c54025bf28583adc6f99bb3c9cb5949a067bc73882b818d35bd6016a78b80
// IndexVersion: 2
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
using System.Linq;
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

            if(!String.IsNullOrEmpty(containerRepo.Password))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(org, containerRepo.Password);
                if (!addSecretResult.Successful) return addSecretResult.ToInvokeResult();
                containerRepo.SecurePasswordId = addSecretResult.Result;
                containerRepo.Password = null;
            }

            await _repo.AddContainerRepoAsync(containerRepo);

            return InvokeResult.Success;
        }

        public async Task<ContainerRepository> RegisterContainerTagAsync(string orgId, string containerName, TaggedContainer taggedContainer)
        {
            if (String.IsNullOrWhiteSpace(orgId)) throw new ArgumentException("Organization id is required.", nameof(orgId));
            if (String.IsNullOrWhiteSpace(containerName)) throw new ArgumentException("Container name is required.", nameof(containerName));
            if (taggedContainer == null) throw new ArgumentNullException(nameof(taggedContainer));
            if (String.IsNullOrWhiteSpace(taggedContainer.Tag)) throw new ArgumentException("Container tag is required.", nameof(taggedContainer));

            var normalizedContainerName = containerName.Trim().Trim('/').ToLowerInvariant();
            var separator = normalizedContainerName.IndexOf('/');
            if (separator <= 0 || separator == normalizedContainerName.Length - 1)
                throw new ArgumentException("Container name must be in namespace/repository form.", nameof(containerName));

            var containerNamespace = normalizedContainerName.Substring(0, separator);
            var repositoryName = normalizedContainerName.Substring(separator + 1);
            if (repositoryName.Contains("/"))
                throw new ArgumentException("Container name must contain exactly one namespace separator.", nameof(containerName));

            var container = await _repo.GetContainerRepoByNameAsync(orgId.Trim(), containerNamespace, repositoryName);
            if (container == null)
            {
                var key = $"{containerNamespace}-{repositoryName}".Replace("_", "-").Replace(".", "-");
                container = new ContainerRepository()
                {
                    Id = Guid.NewGuid().ToString("N").ToUpperInvariant(),
                    Name = normalizedContainerName,
                    Key = key,
                    Namespace = containerNamespace,
                    RepositoryName = repositoryName,
                    OwnerOrganization = EntityHeader.Create(orgId.Trim(), orgId.Trim())
                };
                await _repo.AddContainerRepoAsync(container);
            }

            var existingTag = container.Tags.FirstOrDefault(tag => String.Equals(tag.Tag, taggedContainer.Tag, StringComparison.OrdinalIgnoreCase));
            if (existingTag != null) container.Tags.Remove(existingTag);
            container.Tags.Insert(0, taggedContainer);
            container.PreferredTag = taggedContainer.ToEntityHeader();
            await _repo.UpdateContainerRepoAsync(container);
            return container;
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

            if (String.IsNullOrWhiteSpace(containerRepo.UserName) || String.IsNullOrWhiteSpace(containerRepo.SecurePasswordId))
                throw new InvalidOperationException("Registry credentials are not configured for this container repository.");

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