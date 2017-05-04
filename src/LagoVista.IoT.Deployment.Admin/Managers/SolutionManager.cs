using System.Collections.Generic;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core.Validation;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using static LagoVista.Core.Models.AuthorizeResult;
using LagoVista.IoT.Pipeline.Admin.Managers;
using System;
using LagoVista.IoT.Logging.Exceptions;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Pipeline.Admin.Models;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class SolutionManager : ManagerBase, ISolutionManager
    {
        ISolutionRepo _deploymentRepo;
        IDeviceConfigurationManager _deviceConfigManager;
        IPipelineModuleManager _pipelineModuleManager;

        public SolutionManager(ISolutionRepo deploymentRepo, IDeviceConfigurationManager deviceConfigManager, IPipelineModuleManager pipelineModuleManager,
        ILogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
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
            await AuthorizeAsync(solution, AuthorizeActions.Read, org, user);
            return await CheckForDepenenciesAsync(solution);
        }

        public async Task<InvokeResult> DeleteSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var solution = await GetSolutionAsync(id, org, user);
            await AuthorizeAsync(solution, AuthorizeActions.Delete, org, user);
            await ConfirmNoDepenenciesAsync(solution);
            await _deploymentRepo.DeleteSolutionAsync(solution.Id);

            return InvokeResult.Success;
        }

        public async Task<Solution> GetSolutionAsync(string id, EntityHeader org, EntityHeader user)
        {
            var deployment = await _deploymentRepo.GetSolutionAsync(id);
            await AuthorizeAsync(deployment, AuthorizeActions.Read, org, user);
            return deployment;
        }

        public async Task<Solution> LoadFullSolutionAsync(string id)
        {
            var deployment = await _deploymentRepo.GetSolutionAsync(id);

            foreach (var config in deployment.DeviceConfigurations)
            {
                config.Value = await _deviceConfigManager.LoadFullDeviceConfigurationAsync(config.Id);
            }

            if (deployment.Planner.IsEmpty()) throw InvalidConfigurationException.FromErrorCode(DeploymentErrorCodes.NoPlannerSpecified);

            deployment.Planner.Value = await _pipelineModuleManager.LoadFullPlannerConfigurationAsync(deployment.Planner.Id);

            foreach (var listenerConfig in deployment.Listeners)
            {
                listenerConfig.Value = await _pipelineModuleManager.LoadFullListenerConfigurationAsync(listenerConfig.Id);
            }

            return deployment;
        }


        public async Task<IEnumerable<SolutionSummary>> GetSolutionsForOrgsAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccess(user, orgId, typeof(Solution));
            return await _deploymentRepo.GetSolutionsForOrgsAsync(orgId);
        }

        public async Task<InvokeResult> UpdateSolutionsAsync(Solution deployment, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(deployment, AuthorizeActions.Update, org, user);

            var result = Validator.Validate(deployment, Actions.Update);
            await _deploymentRepo.UpdateSolutionAsync(deployment);

            return result.ToActionResult();
        }

        public Task<bool> QueryKeyInUse(string key, EntityHeader org)
        {
            return _deploymentRepo.QueryKeyInUseAsync(key, org.Id);
        }
    }
}
