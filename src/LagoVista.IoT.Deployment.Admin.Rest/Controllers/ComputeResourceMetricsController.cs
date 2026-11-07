using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    /// <summary>
    /// Read-only history of MCP-collected deployment host compute metrics.
    /// </summary>
    [ConfirmedUser]
    [OrgAdmin]
    [Authorize]
    public class ComputeResourceMetricsController : LagoVistaBaseController
    {
        private readonly IDeploymentHostManager _hostManager;
        private readonly IComputeResourceMetricsHistoryRepo _historyRepo;

        public ComputeResourceMetricsController(
            IDeploymentHostManager hostManager,
            IComputeResourceMetricsHistoryRepo historyRepo,
            UserManager<AppUser> userManager,
            IAdminLogger logger) : base(userManager, logger)
        {
            _hostManager = hostManager;
            _historyRepo = historyRepo;
        }

        /// <summary>
        /// Deployment Host - Get compute resource metrics history
        /// </summary>
        [HttpGet("/api/deployment/host/{id}/compute-metrics")]
        public async Task<ListResponse<ComputeResourceMetrics>> GetForHostAsync(string id)
        {
            // Host manager enforces ownership or system-administrator access before any
            // Cassandra history is queried, so no caller can enumerate another host.
            await _hostManager.GetDeploymentHostAsync(id, OrgEntityHeader, UserEntityHeader);
            return await _historyRepo.GetMetricsForHostAsync(id, GetListRequestFromHeader());
        }
    }
}
