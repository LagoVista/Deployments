using LagoVista.Core.Validation;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using LagoVista.IoT.DeviceManagement.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{

    /// <summary>
    /// Send properties to the remote devices for configuration.
    /// </summary>
    [ConfirmedUser]
    [Authorize]
    public class RemoteConfigurationController : LagoVistaBaseController
    {
        IRemoteConfigurationManager _remoteConfigMgr;

        public RemoteConfigurationController(IRemoteConfigurationManager remoteConfigMgr, UserManager<AppUser> userManager, IAdminLogger logger)
            : base(userManager, logger)
        {
            _remoteConfigMgr = remoteConfigMgr;
        }

        [HttpGet("/api/device/remoteconfig/{repoid}/{id}/all/send")]
        public Task<InvokeResult> SendAllRemotePropertiesAsync(string repoid, string id)
        {
            return _remoteConfigMgr.SendAllPropertiesAsync(repoid, id, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/remoteconfig/{repoid}/{id}/{idx}/send")]
        public Task<InvokeResult> SendRemotePropertyAsync(string repoid, string id, int idx)
        {
            return _remoteConfigMgr.SendPropertyAsync(repoid, id, idx, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/remoteconfig/{repoid}/{id}/query")]
        public Task<InvokeResult> QueryRemoteConfigAsync(string repoid, string id)
        {
            return _remoteConfigMgr.QueryRemoteConfigurationAsync(repoid, id, OrgEntityHeader, UserEntityHeader);
        }

        [HttpPost("/api/device/remoteconfig/{repoid}/{id}/command/{cmdid}")]
        public Task<InvokeResult> SendRemoteCommand(string repoid, string id, string cmdid, [FromBody] List<KeyValuePair<string, string>> parameters)
        {
            return _remoteConfigMgr.SendCommandAsync(repoid, id, cmdid, parameters, OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/device/remoteconfig/{repoid}/{id}/restart")]
        public Task<InvokeResult> RestartDeviceAsync(string repoid, string id)
        {
            return _remoteConfigMgr.RestartDeviceAsync(repoid, id, OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/device/remoteconfig/{repoid}/{deviceid}/firmware/{firmwareid}/revision/{revisionid}")]
        public Task<InvokeResult<string>> ApplyFirmwareAsync(string repoid, string deviceid, string firmwareid, string revisionid, bool triggeredRemotely = false)
        {
            return _remoteConfigMgr.ApplyFirmwareAsync(repoid, deviceid, firmwareid, revisionid, triggeredRemotely, OrgEntityHeader, UserEntityHeader);
        }
    }
}
