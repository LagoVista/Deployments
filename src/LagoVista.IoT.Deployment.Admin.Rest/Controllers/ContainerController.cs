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

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
   
    public class ContainerController : LagoVistaBaseController
    {
        IContainerManager _containerManager;

        public ContainerController(IContainerManager containerManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _containerManager = containerManager;
        }

        /// <summary>
        /// Deployment Config - Add New
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/container")]
        public Task<InvokeResult> AddSolutionAsync([FromBody] Container container)
        {
            return _containerManager.AddContainerAsync(container, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Containers - Update Container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [HttpPut("/api/deployment/solution")]
        public Task<InvokeResult> UpdateContainerAsync([FromBody] Container container)
        {
            SetUpdatedProperties(container);
            return _containerManager.UpdateContainerAsync(container, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Containers - Get Configs for Org
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>        
        [HttpGet("/api/org/{id}/deployment/containers")]
        public async Task<ListResponse<ContainerSummary>> GetContainersForOrgAsync(String id)
        {
            var containers = await _containerManager.GetContainersForOrgAsync(id, UserEntityHeader);
            return ListResponse<ContainerSummary>.Create(containers);
        }

        /// <summary>
        /// Containers - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/container/factory")]
        public DetailResponse<Container> CreateContainer()
        {
            var response = DetailResponse<Container>.Create();
            response.Model.Id = Guid.NewGuid().ToId();
            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }
    }
}
