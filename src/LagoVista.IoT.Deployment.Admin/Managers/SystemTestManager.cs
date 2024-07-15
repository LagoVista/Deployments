using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Threading.Tasks;
using LagoVista.Core.Managers;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core;
using LagoVista.Core.Exceptions;
using System.Linq;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    internal class SystemTestManager : ManagerBase, ISystemTestManager
    {
        private readonly ISystemTestRepo _systemTestsRepo;
        private readonly ISystemTestExecutionRepo _systemTestExecutionRepo;

        public SystemTestManager(ISystemTestRepo systemTestsRepo, ISystemTestExecutionRepo systemTestExecutionRepo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) :
            base(logger, appConfig, dependencyManager, security)
        {
            _systemTestsRepo = systemTestsRepo ?? throw new ArgumentNullException(nameof(systemTestsRepo));
            _systemTestExecutionRepo = systemTestExecutionRepo ?? throw new ArgumentNullException(nameof(systemTestExecutionRepo));
        }

        public async Task<InvokeResult> AddSystemTestAsync(SystemTest systemTest, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(systemTest, Actions.Create);
            await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _systemTestsRepo.AddSystemTestAsync(systemTest);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteSystemTestAsync(string id, EntityHeader org, EntityHeader user)
        {
            var systemTest = await _systemTestsRepo.GetSystemTestAsync(id);
            await ConfirmNoDepenenciesAsync(systemTest);

            await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Delete, user, org);

            await _systemTestsRepo.DeleteSystemTestAsync(id);

            return InvokeResult.Success;
        }

        public async Task<SystemTest> GetSystemTestAsync(string id, EntityHeader org, EntityHeader user)
        {
            var systemTest = await _systemTestsRepo.GetSystemTestAsync(id);
            await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Read, user, org);

            return systemTest;
        }

        public async Task<SystemTest> GetSystemTestByKeyAsync(string key, EntityHeader org, EntityHeader user)
        {
            var systemTest = await _systemTestsRepo.GetSystemTestByKeyAsync(key, org.Id);
            await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Read, user, org);
            return systemTest;
        }

        public async Task<ListResponse<SystemTestSummary>> GetSystemTestsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(SystemTest));
            return await _systemTestsRepo.GetSystemTestsForOrgAsync(orgId, listRequest);
        }

        public async Task<InvokeResult> UpdateSystemTestAsync(SystemTest systemTest, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(systemTest, Actions.Create);
            await AuthorizeAsync(systemTest, AuthorizeResult.AuthorizeActions.Update, user, org);

            await _systemTestsRepo.UpdateSystemTestAsync(systemTest);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult<SystemTestExecution>> StartTestAsync(string systemTestId, EntityHeader org, EntityHeader user)
        {
            var test = await _systemTestsRepo.GetSystemTestAsync(systemTestId);
            var timeStamp = DateTime.UtcNow.ToJSONString();

            var result = new SystemTestExecution()
            {
                StartedBy = user,
                StartTimeStamp = timeStamp,
                CreatedBy = user,
                LastUpdatedBy = user,
                Icon = test.Icon,
                CreationDate = timeStamp,
                LastUpdatedDate = timeStamp,
                OwnerOrganization = org,
                SystemTest = EntityHeader.Create(test.Id, test.Key, test.Name),
                Name = $"{test.Name} {DateTime.Now.ToString()}" ,
                Key = $"{test.Key}{DateTime.Now.Year}{DateTime.Now.Month:00}{DateTime.Now.Day:00}{DateTime.Now.Hour:00}{DateTime.Now.Minute:00}"
            };

            var idx = 0;

            foreach(var step in test.Steps)
            {
                var stepResult = new SystemTestStepResult()
                {
                    Index = idx++,
                    Id = Guid.NewGuid().ToId(),
                    SystemTestStep = EntityHeader.Create(step.Id, step.Key, step.Name)
                };

                result.StepResults.Add(stepResult);
            }

            await _systemTestExecutionRepo.AddSystemTestExecutionAsync(result);

            await AuthorizeAsync(user, org, typeof(SystemTestExecution), Actions.Update, result.Id);

            return InvokeResult<SystemTestExecution>.Create(result);
        }

        public async Task<InvokeResult<SystemTestExecution>> CompleteStepAsync(string testExecutionId, string stepResultId, TestStepUpdate update, EntityHeader org, EntityHeader user)
        {
            if(!update.Passed && String.IsNullOrEmpty(update.ReasonFailed))
            {
                return InvokeResult<SystemTestExecution>.FromError("If the step failed, you must provide a reason.");
            }

            var timeStamp = DateTime.UtcNow.ToJSONString();

            var execution = await _systemTestExecutionRepo.GetSystemTestExecutionAsync(testExecutionId);
            if (execution.Status.Value == TestExecutionStates.New)
            {
                execution.Status = EntityHeader<TestExecutionStates>.Create(TestExecutionStates.InProcess);
            }

            var inProcess = execution.StepResults.Any(step => !step.Passed.HasValue);
            if(!inProcess)
            {
                var failed = execution.StepResults.Any(step => step.Passed.HasValue && !step.Passed.Value);

                execution.Status = EntityHeader<TestExecutionStates>.Create(failed ? TestExecutionStates.Failed : TestExecutionStates.Passed );
                execution.CompletedBy = user;
                execution.EndTimeStamp = timeStamp;
            }

            var stepResult = execution.StepResults.Find(stp => stp.Id == stepResultId);
            if (stepResult == null)
                throw new RecordNotFoundException(nameof(SystemTestStepResult), stepResultId);

            stepResult.Passed = update.Passed;
            stepResult.Notes = update.Notes;
            stepResult.ReasonFailed = update.ReasonFailed;
            stepResult.CompletedBy = user;
            stepResult.TimeStamp = timeStamp;

            execution.LastUpdatedBy = user;
            execution.LastUpdatedDate = timeStamp;

            await _systemTestExecutionRepo.UpdateSystemTestExecutionAsync(execution);

            await AuthorizeAsync(user, org, typeof(SystemTestExecution), Actions.Update, testExecutionId);

            return InvokeResult<SystemTestExecution>.Create(execution);
        }

        public Task<ListResponse<SystemTestExecutionSummary>> GetTestResultsAsync(ListRequest listRequest, EntityHeader org, EntityHeader user)
        {
            return _systemTestExecutionRepo.GetSystemTestExecutionsAsync(org.Id, listRequest);
        }

        public async Task<InvokeResult<SystemTestExecution>> GetTestResultAsync(string systemTestExecutionId, EntityHeader org, EntityHeader user)
        {
            var result = await _systemTestExecutionRepo.GetSystemTestExecutionAsync(systemTestExecutionId);
            return InvokeResult<SystemTestExecution>.Create(result);
        }

        public async Task<InvokeResult<SystemTestExecution>> AbortTestAsync(string testExecutionId, EntityHeader org, EntityHeader user)
        {
            var timeStamp = DateTime.UtcNow.ToJSONString();

            var execution = await _systemTestExecutionRepo.GetSystemTestExecutionAsync(testExecutionId);
            if(execution.Status.Value != TestExecutionStates.InProcess || execution.Status.Value !=  TestExecutionStates.New)
            {
                return InvokeResult<SystemTestExecution>.FromError($"Can not abort a test in the {execution.Status.Text} status.");
            }

            execution.Status = EntityHeader<TestExecutionStates>.Create(TestExecutionStates.Aborted);
            execution.AbortedBy = user;
            execution.EndTimeStamp = timeStamp;

            execution.LastUpdatedBy = user;            
            execution.LastUpdatedDate = timeStamp;

            await AuthorizeAsync(user, org, typeof(SystemTestExecution), Actions.Update, testExecutionId);

            return InvokeResult<SystemTestExecution>.Create(execution);
        }

        public async Task<InvokeResult> DeleteExecutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(user, org, typeof(SystemTestExecution), Actions.Delete, id);

            await _systemTestExecutionRepo.DeleteTestExecutionAsync(id);
            return InvokeResult.Success;
        }
    }
}
