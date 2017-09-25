using System.Collections.Generic;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core.Validation;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using static LagoVista.Core.Models.AuthorizeResult;
using LagoVista.IoT.Pipeline.Admin.Managers;
using System;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class SolutionManager : ManagerBase, ISolutionManager
    {
        ISolutionRepo _deploymentRepo;
        IDeviceConfigurationManager _deviceConfigManager;
        IPipelineModuleManager _pipelineModuleManager;

        public SolutionManager(ISolutionRepo deploymentRepo, IDeviceConfigurationManager deviceConfigManager, IPipelineModuleManager pipelineModuleManager,
        IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _deploymentRepo = deploymentRepo;
            _deviceConfigManager = deviceConfigManager;
            _pipelineModuleManager = pipelineModuleManager;
        }

        public async Task<InvokeResult> AddSolutionsAsync(Solution solution, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(solution, AuthorizeActions.Create, user, org);
            ValidationCheck(solution, Actions.Create);
            await _deploymentRepo.AddSolutionAsync(solution);
            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var solution = await _deploymentRepo.GetSolutionAsync(id);
            await AuthorizeAsync(solution, AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(solution);
        }

        public async Task<InvokeResult> DeleteSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var solution = await GetSolutionAsync(id, org, user);
            await AuthorizeAsync(solution, AuthorizeActions.Delete, user, org);
            await ConfirmNoDepenenciesAsync(solution);
            await _deploymentRepo.DeleteSolutionAsync(solution.Id);

            return InvokeResult.Success;
        }

        public async Task<Solution> GetSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deployment = await _deploymentRepo.GetSolutionAsync(id);
            await AuthorizeAsync(deployment, AuthorizeActions.Read, user, org);
            return deployment;
        }

        public async Task<InvokeResult<Solution>> LoadFullSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var result = new InvokeResult<Solution>();

            var solution = await _deploymentRepo.GetSolutionAsync(id);

            foreach (var config in solution.DeviceConfigurations)
            {

                var loadResult = await _deviceConfigManager.LoadFullDeviceConfigurationAsync(config.Id, org, user);
                if (result.Successful)
                {
                    config.Value = loadResult.Result;
                }
                else
                {
                    result.Concat(loadResult);
                }
            }

            if (solution.Planner.IsEmpty())
            {
                result.Errors.Add(DeploymentErrorCodes.NoPlannerSpecified.ToErrorMessage());
            }
            else
            {
                var loadResult = await _pipelineModuleManager.LoadFullPlannerConfigurationAsync(solution.Planner.Id);
                if (loadResult.Successful)
                {
                    solution.Planner.Value = loadResult.Result;
                }
                else
                {
                    result.Concat(loadResult);
                }
            }

            foreach (var listenerConfig in solution.Listeners)
            {
                 var loadResult = await _pipelineModuleManager.LoadFullListenerConfigurationAsync(listenerConfig.Id);
                if(loadResult.Successful)
                {
                    listenerConfig.Value = loadResult.Result;
                }
                else
                {
                    result.Concat(loadResult);
                }
            }

            return InvokeResult<Solution>.Create(solution);
        }

        public async Task<ValidationResult> ValidationSolution(string id, EntityHeader org, EntityHeader user)
        {
            var result = new ValidationResult();
            try
            {
                var solutionLoadResult = await LoadFullSolutionAsync(id, org, user);
                if (!solutionLoadResult.Successful)
                {
                    return solutionLoadResult;
                }

                var solution = solutionLoadResult.Result;

                if (solution.Listeners.Count == 0) result.Warnings.Add(Resources.DeploymentErrorCodes.NoListeners.ToErrorMessage());
                if (solution.DeviceConfigurations.Count == 0) result.Warnings.Add(Resources.DeploymentErrorCodes.NoDeviceConfigs.ToErrorMessage());

                foreach (var listner in solution.Listeners)
                {
                    if (listner.Value == null)
                    {
                        result.Errors.Add(Resources.DeploymentErrorCodes.CouldNotLoadListener.ToErrorMessage());
                    }
                    else
                    {
                        result.Concat(Validator.Validate(listner.Value));
                    }
                }

                foreach (var couldNotLoadDeviceConfig in solution.DeviceConfigurations)
                {
                    if (couldNotLoadDeviceConfig.Value == null)
                    {
                        result.Errors.Add(Resources.DeploymentErrorCodes.CouldNotLoadDeviceConfiguration.ToErrorMessage());
                    }
                    else
                    {
                        result.Concat(Validator.Validate(couldNotLoadDeviceConfig.Value));
                    }
                }

            }
            catch (Exception ex)
            {
                result.AddSystemError("UNHANDLED EXCEPTION:" + ex.Message);
            }

            return result;
        }

        public async Task<IEnumerable<SolutionSummary>> GetSolutionsForOrgsAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Solution));
            return await _deploymentRepo.GetSolutionsForOrgsAsync(orgId);
        }

        public async Task<InvokeResult> UpdateSolutionsAsync(Solution deployment, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deployment, AuthorizeActions.Update, user, org);

            var result = Validator.Validate(deployment, Actions.Update);
            await _deploymentRepo.UpdateSolutionAsync(deployment);

            return result.ToInvokeResult();
        }

        public Task<bool> QueryKeyInUse(string key, EntityHeader org)
        {
            return _deploymentRepo.QueryKeyInUseAsync(key, org.Id);
        }
    }
}
