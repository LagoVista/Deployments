using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class CloudActivitiesController : LagoVistaBaseController
    {
        private const string MAX_DATE = "9999-12-31T23:59:59.999Z";

        IDeploymentActivityQueueManager _activityManager;
        public CloudActivitiesController(IDeploymentActivityQueueManager activityManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _activityManager = activityManager;
        }

        /// <summary>
        /// Telemetry Get For Host
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/deploymentactivity/active/{id}/{take}/{before?}")]
        public async Task<ListResponse<DeploymentActivity>> GetActiveAsync(string id, int take, string before = MAX_DATE)
        {
            return ListResponse<DeploymentActivity>.Create(await _activityManager.GetActiveActivitiesAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }

        /// <summary>
        /// Telemetry Get For Host
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/deploymentactivity/failed/{id}/{take}/{before?}")]
        public async Task<ListResponse<DeploymentActivity>> GetFailedAsync(string id, int take, string before = MAX_DATE)
        {
            return ListResponse<DeploymentActivity>.Create(await _activityManager.GetFailedActivitiesAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }

        /// <summary>
        /// Telemetry Get For Host
        /// </summary>
        /// <param name="id"></param>
        /// <param name="take"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [HttpGet("/api/deploymentactivity/completed/{id}/{take}/{before?}")]
        public async Task<ListResponse<DeploymentActivity>> GetCompletedAsync(string id, int take, string before = MAX_DATE)
        {
            return ListResponse<DeploymentActivity>.Create(await _activityManager.GetCompletedActivitiesAsync(id, take, before, OrgEntityHeader, UserEntityHeader));
        }
    }
}