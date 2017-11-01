using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Web.Common.Controllers;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.Core.Models;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class SolutionController : LagoVistaBaseController
    {
        ISolutionManager _solutionManager;
        public SolutionController(ISolutionManager deploymentConfigManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _solutionManager = deploymentConfigManager;
        }

        /// <summary>
        /// Deployment Config - Add New
        /// </summary>
        /// <param name="deploymentConfiguration"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/solution")]
        public Task<InvokeResult> AddSolutionAsync([FromBody] Solution deploymentConfiguration)
        {
            return _solutionManager.AddSolutionsAsync(deploymentConfiguration, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Config - Publish 
        /// </summary>
        /// <param name="solutionVersion"></param>
        /// <returns></returns>
        [HttpPost("/api/deployment/solution/publish")]
        public Task<InvokeResult> PublishSolutionAsync([FromBody] SolutionVersion solutionVersion)
        {
            return _solutionManager.PublishSolutionAsync(solutionVersion, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Config - Update Config
        /// </summary>
        /// <param name="deploymentConfiguration"></param>
        /// <returns></returns>
        [HttpPut("/api/deployment/solution")]
        public Task<InvokeResult> UpdateSolutionsConfiguration([FromBody] Solution deploymentConfiguration)
        {
            SetUpdatedProperties(deploymentConfiguration);
            return _solutionManager.UpdateSolutionsAsync(deploymentConfiguration, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Config - Get Configs for Current Org
        /// </summary>
        /// <returns></returns>        
        [HttpGet("/api/deployment/solutions")]
        public async Task<ListResponse<SolutionSummary>> GetSolutionsForOrgAsync()
        {
            var deploymentConfiguration = await _solutionManager.GetSolutionsForOrgsAsync(OrgEntityHeader.Id, UserEntityHeader);
            var response = ListResponse<SolutionSummary>.Create(deploymentConfiguration);

            return response;
        }

        /// <summary>
        /// Deployment Config - Get Versions for Solution Async
        /// </summary>
        /// <returns></returns>        
        [HttpGet("/api/deployment/solution/{id}/versions")]
        public async Task<ListResponse<SolutionVersion>> GetVersionsForSolutionAsync(string id)
        {
            var deploymentConfiguration = await _solutionManager.GetVersionsForSolutionAsync(id, OrgEntityHeader, UserEntityHeader);
            var response = ListResponse<SolutionVersion>.Create(deploymentConfiguration);

            return response;
        }


        /// <summary>
        /// Deployment Config - Get A Configuration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/solution/{id}")]
        public async Task<DetailResponse<Solution>> GetSolutionAsync(String id)
        {
            var deploymentConfiguration = await _solutionManager.GetSolutionAsync(id, OrgEntityHeader, UserEntityHeader);

            var response = DetailResponse<Solution>.Create(deploymentConfiguration);

            return response;
        }

        /// <summary>
        /// Deployment Config - Run Full Validation on a Solution
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/deployment/solution/{id}/validate")]
        public async Task<ValidationResult> ValidateAsync(String id)
        {
            return await _solutionManager.ValidateSolutionAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Deployment Config - Key In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/solution/{key}/InUse")]
        public Task<bool> SolutionKeyInUse(String key)
        {
            return _solutionManager.QueryKeyInUse(key, OrgEntityHeader);
        }

        /// <summary>
        /// Deployment Config - In Use
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/solution/{key}/keyinuse")]
        public Task<DependentObjectCheckResult> SolutionInUse(String id)
        {
            return _solutionManager.CheckInUseAsync(id, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// Device Config - Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/deployment/solution/{id}")]
        public Task<InvokeResult> DeleteSolutionAsync(string id)
        {
            return _solutionManager.DeleteSolutionAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        ///  Deployment Config - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/deployment/solution/factory")]
        public DetailResponse<Solution> CeateDeploymentConfigurartion()
        {
            var response = DetailResponse<Solution>.Create();
            response.Model.Id = Guid.NewGuid().ToId();

            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

    }
}
