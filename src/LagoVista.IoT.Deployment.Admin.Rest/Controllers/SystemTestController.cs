using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class SystemTestController : LagoVistaBaseController
    {
        ISystemTestManager _systemTestManager;

        public SystemTestController(ISystemTestManager systemTestManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _systemTestManager = systemTestManager ?? throw new ArgumentNullException(nameof(systemTestManager));
        }

        [HttpPost("/api/systemtest")]
        public Task<InvokeResult> AddIncidentAsync([FromBody] SystemTest systemTest)
        {
            return _systemTestManager.AddSystemTestAsync(systemTest, OrgEntityHeader, UserEntityHeader);
        }

        [HttpPut("/api/systemtest")]
        public Task<InvokeResult> UpdateIncidentAsync([FromBody] SystemTest systemTest)
        {
            SetUpdatedProperties(systemTest);
            return _systemTestManager.UpdateSystemTestAsync(systemTest, OrgEntityHeader, UserEntityHeader);
        }

        [HttpGet("/api/systemtest/{id}")]
        public async Task<DetailResponse<SystemTest>> GetIncidentAsync(string id)
        {
            var systemTest = await _systemTestManager.GetSystemTestAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<SystemTest>.Create(systemTest);
        }

        /// <summary>
        /// Device Error Code - Create New Template
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/factory")]
        public DetailResponse<SystemTest> CreateSystemTest()
        {
            var systemTest = DetailResponse<SystemTest>.Create();
            SetAuditProperties(systemTest.Model);
            SetOwnedProperties(systemTest.Model);
            return systemTest;
        }

        /// <summary>
        /// Device Error Code - Create New Template
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/step/factory")]
        public DetailResponse<SystemTestStep> CreateSystemTestStep()
        {
            return DetailResponse<SystemTestStep>.Create();
        }


        /// <summary>
        /// Device Error Code - Get For Organization
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtests")]
        public Task<ListResponse<SystemTestSummary>> GetSystemTestsForOrg()
        {
            return _systemTestManager.GetSystemTestsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }

        /// <summary>
        /// Device Error Code - Delete Device Error Code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/systemtest/{id}")]
        public async Task<InvokeResult> DeleteSystemTest(string id)
        {
            return await _systemTestManager.DeleteSystemTestAsync(id, OrgEntityHeader, UserEntityHeader);
        }
    }
}