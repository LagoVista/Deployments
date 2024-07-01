using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class DeviceErrorCodesController : LagoVistaBaseController
    {
        IDeviceErrorCodesManager _errorCodeManager;

        public DeviceErrorCodesController(IDeviceErrorCodesManager errorCodeManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _errorCodeManager = errorCodeManager ?? throw new ArgumentNullException(nameof(errorCodeManager));   
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        [HttpPost("/api/errorcode")]
        public Task<InvokeResult> AddErrorCodeAsync([FromBody] DeviceErrorCode errorCode)
        {
            return _errorCodeManager.AddErrorCodeAsync(errorCode, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Add New
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        [HttpPut("/api/errorcode")]
        public Task<InvokeResult> UpdateErrorCodeAsync([FromBody] DeviceErrorCode errorCode)
        {
            SetUpdatedProperties(errorCode);
            return _errorCodeManager.UpdateErrorCodeAsync(errorCode, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Device Error Code - Get Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/errorcode/{id}")]
        public async Task<DetailResponse<DeviceErrorCode>> GetErrorCodeAsync(string id)
        {
            var container = await _errorCodeManager.GetErrorCodeAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<DeviceErrorCode>.Create(container);
        }

        /// <summary>
        /// Device Error Code - Create New Template
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/errorcode/factory")]
        public DetailResponse<DeviceErrorCode> CreateNewErrorCode()
        {
            var errorCode = DetailResponse<DeviceErrorCode>.Create();
            SetAuditProperties(errorCode.Model);
            SetOwnedProperties(errorCode.Model);
            return errorCode;
        }

        /// <summary>
        /// Device Error Code - Get For Organization
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/errorcodes")]
        public Task<ListResponse<DeviceErrorCodeSummary>> GetErrorCodesForOrg()
        {
            return _errorCodeManager.GetErrorCodesForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Device Error Code - Delete Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/errorcode/{id}")]
        public async Task<InvokeResult> DeleteErrorCodeAsync(string id)
        {
            return await _errorCodeManager.DeleteErrorCodeAsync(id, OrgEntityHeader, UserEntityHeader);
        }
    }
}
