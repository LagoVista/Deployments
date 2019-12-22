using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceAdmin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public Task<InvokeResult> SendAllRemotePropertiesAsync(string repod, string id)
        {
            return _remoteConfigMgr.SendAllPropertiesAsync(repod, id, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/remoteconfig/{repoid}/{id}/{idx}/send")]
        public Task<InvokeResult> SendRemotePropertyAsync(string repod, string id, int idx)
        {
            return _remoteConfigMgr.SendPropertyAsync(repod, id, idx, OrgEntityHeader, UserEntityHeader);
        }


        [HttpGet("/api/device/remoteconfig/{repoid}/{id}")]
        public Task<ListResponse<AttributeValue>> GetAllPropertiesAsync(string repoid, string id)
        {
            return _remoteConfigMgr.GetAllPropertiesAsync(repoid, id, OrgEntityHeader, UserEntityHeader);
        }
    }
}
