// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f85fda4969506d0f35e91967a76a3fadcf74af6b5b947b1c9d77e6f81d9e8a03
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Web.Common.Controllers;
using Microsoft.AspNetCore.Authorization;
using System;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using LagoVista.IoT.Deployment.Admin.Managers;
using Microsoft.AspNetCore.Mvc;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Validation;
using System.Threading.Tasks;
using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;
using LagoVista.Core.Exceptions;
using Newtonsoft.Json;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize(AuthenticationSchemes = "APIToken")]
    public class ContainerRepositoryControllerClientAPI : LagoVistaBaseController
    {
        IContainerRepositoryManager _containerManager;

        public ContainerRepositoryControllerClientAPI(IContainerRepositoryManager containerManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _containerManager = containerManager;
        }

        [HttpPost("/clientapi/container/{id}/tag")]
        public async Task AddImageToContainer(string id, [FromBody] TaggedContainer taggedContainer)
        {
            if (taggedContainer == null)
                throw new ArgumentNullException(nameof(taggedContainer));

            var json = JsonConvert.SerializeObject(taggedContainer);
            Console.WriteLine(json);

            var container = await _containerManager.GetContainerRepoAsync(id, OrgEntityHeader, UserEntityHeader);
            if(container == null)
            {
                throw new RecordNotFoundException(nameof(ContainerRepository), id);
            }
            container.Tags.Insert(0, taggedContainer);
            container.PreferredTag = Core.Models.EntityHeader.Create(taggedContainer.Id, taggedContainer.Name);
            await _containerManager.UpdateContainerRepoAsync(container, OrgEntityHeader, UserEntityHeader);
        }
    }


    [Authorize]

    public class ContainerRepositoryController : LagoVistaBaseController
    {
        IContainerRepositoryManager _containerManager;

        public ContainerRepositoryController(IContainerRepositoryManager containerManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _containerManager = containerManager;
        }

        /// <summary>
        /// Container Repo - Add New
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [HttpPost("/api/container/repo")]
        public Task<InvokeResult> AddSolutionAsync([FromBody] ContainerRepository container)
        {
            return _containerManager.AddContainerRepoAsync(container, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Container Repo - Get Container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/container/repo/{id}")]
        public async Task<DetailResponse<ContainerRepository>> GetContainerAsync(string id)
        {
            var container = await _containerManager.GetContainerRepoAsync(id, OrgEntityHeader, UserEntityHeader);
            var detailResponse = DetailResponse<ContainerRepository>.Create(container);
            detailResponse.View["password"].IsRequired = false;
            return detailResponse;
        }

        /// <summary>
        /// Container Repo - Update Container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [HttpPut("/api/container/repo")]
        public Task<InvokeResult> UpdateContainerAsync([FromBody] ContainerRepository container)
        {
            SetUpdatedProperties(container);
            return _containerManager.UpdateContainerRepoAsync(container, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Container Repo - Get Repos for Current Org
        /// </summary>
        /// <returns></returns>        
        [HttpGet("/api/container/repos")]
        public async Task<ListResponse<ContainerRepositorySummary>> GetContainersForOrgAsync()
        {
            var containerRepos = await _containerManager.GetContainerReposForOrgAsync(OrgEntityHeader.Id, UserEntityHeader);
            return ListResponse<ContainerRepositorySummary>.Create(containerRepos);
        }

        /// <summary>
        /// Container Repo - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/container/repo/factory")]
        public DetailResponse<ContainerRepository> CreateContainerRepository()
        {
            var response = DetailResponse<ContainerRepository>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            response.View["password"].IsRequired = true;
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Tagged Container - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/container/tag/factory")]
        public DetailResponse<TaggedContainer> CreateTaggedContainer()
        {
            return DetailResponse<TaggedContainer>.Create();
        }

        /// <summary>
        /// Tagged Container - Get tags registered for use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/container/{id}/tags")]
        public async Task<ListResponse<TaggedContainer>> GetTagsForContainer(string id)
        {
            var container = await _containerManager.GetContainerRepoAsync(id, OrgEntityHeader, UserEntityHeader);
            return ListResponse<TaggedContainer>.Create(container.Tags);
        }

        /// <summary>
        /// Tagged Container - Get tags from remote repository
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/container/{id}/registry/tags")]
        public async Task<ListResponse<DockerTag>> GetTagsFromRegistryAsync(string id)
        {
            var container = await _containerManager.GetTagsFromRemoteRegistryAsync(id, OrgEntityHeader, UserEntityHeader);
            return ListResponse<DockerTag>.Create(container);
        }
    }
}
