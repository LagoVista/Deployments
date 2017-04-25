using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.Core.Managers;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Authentication.Exceptions;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentInstanceManager : ManagerBase, IDeploymentInstanceManager
    {
        IDeploymentInstanceRepo _instanceRepo;
        ISolutionManager _solutionManager;

        public DeploymentInstanceManager(IDeploymentInstanceRepo instanceRepo, ISolutionManager deploymentConfigurationManager, ILogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _instanceRepo = instanceRepo;
            _solutionManager = deploymentConfigurationManager;
        }


        public async Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(instance, Actions.Create);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _instanceRepo.AddInstanceAsync(instance);

            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUserAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(instance);
        }

        public async Task<InvokeResult> DeleteInstanceAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);
            await CheckForDepenenciesAsync(instance);


            await _instanceRepo.DeleteInstanceAsync(instanceId);

            return InvokeResult.Success; ;
        }

        public async Task<DeploymentInstance> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

            return instance;
        }

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsyncAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccess(user, orgId);
            return await _instanceRepo.GetInstanceForOrgAsyncAsync(orgId);
        }

        public async Task<DeploymentInstance> LoadFullInstanceAsync(string id)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);

            instance.Solution.Value = await _solutionManager.LoadFullSolutionAsync(id);
            instance.Solution.Id = instance.Solution.Value.Id;
            instance.Solution.Text = instance.Solution.Value.Name;

            return instance;

        }

        public Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org)
        {
            return _instanceRepo.QueryInstanceKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance, EntityHeader user)
        {
            ValidationCheck(instance, Actions.Update);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Update, user);

            await _instanceRepo.UpdateInstanceAsync(instance);

            return InvokeResult.Success;
        }
    }
}
