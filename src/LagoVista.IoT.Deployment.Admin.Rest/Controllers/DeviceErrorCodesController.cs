// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 43f1543631b93fda5568f329418e6bc02c61e725151a78bacb79bdac9f84845a
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class DeviceErrorCodesController : LagoVistaBaseController
    {
        private readonly IDeviceErrorCodesManager _errorCodeManager;
        private readonly IDeviceErrorHandler _deviceErrorHandler;

        public DeviceErrorCodesController(IDeviceErrorCodesManager errorCodeManager, IDeviceErrorHandler deviceErrorHandler, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _errorCodeManager = errorCodeManager ?? throw new ArgumentNullException(nameof(errorCodeManager));   
            _deviceErrorHandler = deviceErrorHandler ?? throw new ArgumentNullException(nameof(deviceErrorHandler));
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

        [HttpGet("/api/device/{repoid}/{id}/error/{errorcode}/raise")]
        public async Task<InvokeResult> RaiseError(string repoid, string id, string errorcode)
        {
            return await _deviceErrorHandler.HandleDeviceExceptionAsync(new DeviceManagement.Models.DeviceException()
            {
                 DeviceUniqueId = id,
                 DeviceRepositoryId = repoid,
                 ErrorCode = errorcode,
                 DeviceId = "TBD",
                 Timestamp = DateTime.UtcNow.ToJson(),

            }, OrgEntityHeader, UserEntityHeader);
        }
    }
}
