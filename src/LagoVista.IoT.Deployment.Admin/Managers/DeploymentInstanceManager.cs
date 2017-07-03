//#define WEBSERVERSIDECHECK

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
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.DeviceManagement.Core.Managers;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentInstanceManager : ManagerBase, IDeploymentInstanceManager
    {

        public const string DeploymentAction_Deploy = "Instance.Deploy";
        public const string DeploymentAction_Start = "Instance.Start";
        public const string DeploymentAction_Pause = "Instance.Pause";
        public const string DeploymentAction_Stop = "Instance.Stop";
        public const string DeploymentAction_Remove = "Instance.Remove";
        public const string DeploymentAction_Monitor = "Instance.Monitor";

        IDeploymentInstanceRepo _instanceRepo;
        ISolutionManager _solutionManager;
        IDeploymentHostManager _hostManager;
        IDeploymentConnectorService _connector;
        IDeviceRepositoryManager _deviceRepoManager;

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager deploymentConfigurationManager,
                     IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _hostManager = hostManager;
            _instanceRepo = instanceRepo;
            _solutionManager = deploymentConfigurationManager;
            _connector = connector;
            _deviceRepoManager = deviceRepoManager;
        }

        public async Task<InvokeResult> DeployAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);
#if WEBSERVERSIDECHECK
            if (instance.IsDeployed)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.AlreadyDeployed);
            }

            if (instance.Host.IsEmpty())
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceWithoutHost);
            }

            if (instance.Solution.IsEmpty())
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceWithoutSolution);
            }
#endif

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Perform, user, org, DeploymentAction_Deploy);

            var host = await _hostManager.GetDeploymentHostAsync(instance.Host.Id, org, user);
            return await _connector.DeployAsync(host, id, org, user);
        }

        public async Task<InvokeResult> StartAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

#if WEBSERVERSIDECHECK
            if (!instance.IsDeployed)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.NotDeployed);
            }

            if (instance.Status.Id != DeploymentInstance.Status_Stopped && instance.Status.Id != DeploymentInstance.Status_Running)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceAlreadyRunning);
            }

            if (instance.Status.Id != DeploymentInstance.Status_Stopped && instance.Status.Id != DeploymentInstance.Status_Paused)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceBusy);
            }
#endif

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Perform, user, org, DeploymentAction_Start);

            var host = await _hostManager.GetDeploymentHostAsync(instance.Host.Id, org, user);
            return await _connector.StartAsync(host, id, org, user);
        }

        public async Task<InvokeResult> PauseAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

#if WEBSERVERSIDECHECK
            if (!instance.IsDeployed)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.NotDeployed);
            }

            if (instance.Status.Id != DeploymentInstance.Status_Stopped && instance.Status.Id != DeploymentInstance.Status_Running)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceNotRunning);
            }

            if (instance.Status.Id != DeploymentInstance.Status_Running && instance.Status.Id != DeploymentInstance.Status_Paused)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceBusy);
            }
#endif


            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Perform, user, org, DeploymentAction_Pause);

            var host = await _hostManager.GetDeploymentHostAsync(instance.Host.Id, org, user);
            return await _connector.PauseAsync(host, id, org, user);
        }

        public async Task<InvokeResult> StopAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

#if WEBSERVERSIDECHECK
            if (!instance.IsDeployed)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.NotDeployed);
            }

            if (instance.Status.Id != DeploymentInstance.Status_Stopped && instance.Status.Id != DeploymentInstance.Status_Running)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceNotRunning);
            }

            if (instance.Status.Id != DeploymentInstance.Status_Running && instance.Status.Id != DeploymentInstance.Status_Paused)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.InstanceBusy);
            }
#endif

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Perform, user, org, DeploymentAction_Stop);

            var host = await _hostManager.GetDeploymentHostAsync(instance.Host.Id, org, user);
            return await _connector.StopAsync(host, id, org, user);
        }

        public async Task<InvokeResult> RemoveAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

#if WEBSERVERSIDECHECK
            if (!instance.IsDeployed)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.NotDeployed);
            }

            if (instance.Status.Id != DeploymentInstance.Status_Stopped && instance.Status.Id != DeploymentInstance.Status_FatalError)
            {
                throw LagoVista.IoT.Logging.Exceptions.InvalidOperationException.FromErrorCode(Resources.DeploymentErrorCodes.MustBeStoppedBeforeRemoving);
            }
#endif

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Perform, user, org, DeploymentAction_Remove);

            var host = await _hostManager.GetDeploymentHostAsync(instance.Host.Id, org, user);
            return await _connector.RemoveAsync(host, id, org, user);
        }

        public async Task<InvokeResult<string>> GetRemoteMonitoringURIAsync(string channel, string id, string verbosity, EntityHeader org, EntityHeader user)
        {
            var host = await _hostManager.GetNotificationsHostAsync(org, user);
            return await _connector.GetRemoteMonitoringUriAsync(host, channel, id, verbosity, org, user);
        }
       
        public async Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(instance, Actions.Create);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _instanceRepo.AddInstanceAsync(instance);

            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
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

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId);
            return await _instanceRepo.GetInstanceForOrgAsync(orgId);
        }

        public async Task<DeploymentInstance> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

            instance.DeviceRepository.Value = await _deviceRepoManager.GetDeviceRepositoryAsync(instance.DeviceRepository.Id, org, user);

            instance.Solution.Value = await _solutionManager.LoadFullSolutionAsync(instance.Solution.Id, org, user);
            instance.Solution.Id = instance.Solution.Value.Id;
            instance.Solution.Text = instance.Solution.Value.Name;

            return instance;
        }

        public Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org)
        {
            return _instanceRepo.QueryInstanceKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(instance, Actions.Update);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Update, org, user);

            await _instanceRepo.UpdateInstanceAsync(instance);

            return InvokeResult.Success;
        }
    }
}
