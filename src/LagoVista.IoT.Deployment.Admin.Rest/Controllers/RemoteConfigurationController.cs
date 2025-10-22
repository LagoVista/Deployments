// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 243520d2e4a5643fd3075badff98dfbd0ec3a82556fb6fd91a2b0748fc07d990
// IndexVersion: 0
// --- END CODE INDEX META ---
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
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.Core.Models;
using System;
using LagoVista.Core;

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
        IDeviceRepositoryManager _repoManager;
        IDeviceManager _deviceManager;

        public RemoteConfigurationController(IRemoteConfigurationManager remoteConfigMgr, IDeviceRepositoryManager repoManager, IDeviceManager deviceManager, UserManager<AppUser> userManager, IAdminLogger logger)
            : base(userManager, logger)
        {
            _remoteConfigMgr = remoteConfigMgr;
            _repoManager = repoManager;
            _deviceManager = deviceManager;
        }

        [HttpGet("/api/device/remoteconfig/{orgid}/{repoid}/{id}/{pin}/all/send")]
        public async Task<InvokeResult> SendAllRemotePropertiesAsync(string orgid, string repoid, string id, string pin)
        {
            var org = EntityHeader.Create(orgid, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(repoid, org, user, pin);
            var result = await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user);
            if (result.Successful)
                return await _remoteConfigMgr.SendAllPropertiesAsync(repoid, id, org, user);
            else
                return result.ToInvokeResult();
        }

        [HttpGet("/api/device/remoteconfig/{repoid}/{id}/all/send")]
        public async Task<InvokeResult> SendAllRemotePropertiesAsync(string repoid, string id)
        {
            return await _remoteConfigMgr.SendAllPropertiesAsync(repoid, id, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/remoteconfig/{repoid}/{id}/{idx}/send")]
        public async Task<InvokeResult> SendRemotePropertyAsync(string repoid, string id, int idx)
        {
            return await _remoteConfigMgr.SendPropertyAsync(repoid, id, idx, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/remoteconfig/{orgid}/{repoid}/{id}/{pin}/{idx}/send")]
        public async Task<InvokeResult> SendRemotePropertyAsync(string orgid, string repoid, string id, string pin, int idx)
        {
            var org = EntityHeader.Create(orgid, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");

            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(repoid, org, user, pin);
            var result = await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user);
            if (result.Successful)
                return await _remoteConfigMgr.SendPropertyAsync(repoid, id, idx, org, user);
            else
                return result.ToInvokeResult();
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

        [HttpPost("/api/device/remoteconfig/{orgid}/{repoid}/{id}/{pin}/command/{cmdid}")]
        public async Task<InvokeResult> SendRemoteCommand(string orgid, string repoid, string id, string pin, string cmdid, [FromBody] List<KeyValuePair<string, string>> parameters)
        {
            var org = EntityHeader.Create(orgid, "PIN Device Access");
            var user = EntityHeader.Create(Guid.Empty.ToId(), "PIN Device Access");


            var repo = await _repoManager.GetDeviceRepositoryWithSecretsAsync(repoid, org, user, pin);
            var result = await _deviceManager.GetDeviceByIdWithPinAsync(repo, id, pin, org, user);
            if (result.Successful)
                return await _remoteConfigMgr.SendCommandAsync(repoid, id, cmdid, parameters, org, user);
            else
                return result.ToInvokeResult();
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
