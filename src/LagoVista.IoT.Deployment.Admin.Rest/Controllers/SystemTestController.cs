// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 78b5f03c951f4e34a37873c4f7aea0dea291f0bc2bd6c8920cc029b7702419be
// IndexVersion: 0
// --- END CODE INDEX META ---
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

        /// <summary>
        /// System Test Execution - Start a system test
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/{id}/start")]
        public Task<InvokeResult<SystemTestExecution>> StartSystemTest(string id)
        {
            return _systemTestManager.StartTestAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// System Test Execution - Start a system test
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/{id}/abort")]
        public Task<InvokeResult<SystemTestExecution>> AbortSystemTest(string id)
        {
            return _systemTestManager.AbortTestAsync(id, OrgEntityHeader, UserEntityHeader);
        }


        /// <summary>
        /// System Test Execution - Start a system test
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/execution/{id}/result")]
        public Task<InvokeResult<SystemTestExecution>> GetSystemTestExecutionResult(string id)
        {
            return _systemTestManager.GetTestResultAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// System Test Execution - Start a system test
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/api/systemtest/execution/{id}")]
        public Task<InvokeResult> DeleteResultAsync(string id)
        {
            return _systemTestManager.DeleteExecutionAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// System Test Execution - Start a system test
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/execution/results")]
        public Task<ListResponse<SystemTestExecutionSummary>> GetSystemTestExecutionResults()
        {
            return _systemTestManager.GetTestResultsAsync(GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// System Test Execution - Start a system test
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/execution/customer/{customerid}/results")]
        public Task<ListResponse<SystemTestExecutionSummary>> GetSystemTestExecutionResults(string customerid)
        {
            return _systemTestManager.GetTestResultsForCustomerAsync(customerid, GetListRequestFromHeader(), OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// System Test Execution - Complete Step
        /// </summary>
        /// <returns></returns>
        [HttpPost("/api/systemtest/{systemTestExecutionId}/step/{stepId}/complete")]
        public Task<InvokeResult<SystemTestExecution>> AddTestStep(string systemTestExecutionId, string stepId, [FromBody] TestStepUpdate update)
        {
            return _systemTestManager.CompleteStepAsync(systemTestExecutionId, stepId, update, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// System Test Execution - Undo Test Step
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/systemtest/{systemTestExecutionId}/step/{stepId}/undo")]
        public Task<InvokeResult<SystemTestExecution>> UndoTestStep(string systemTestExecutionId, string stepId)
        {
            return _systemTestManager.UndoStepAsync(systemTestExecutionId, stepId, OrgEntityHeader, UserEntityHeader);
        }
    }
}