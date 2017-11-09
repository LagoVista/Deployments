//#define WEBSERVERSIDECHECK

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.Core;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.DeviceManagement.Core.Managers;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentInstanceManager : DeploymentInstanceManagerCore, IDeploymentInstanceManager
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
        IDeploymentHostRepo _hostRepo;
        IDeploymentConnectorService _connector;
        IDeviceRepositoryManager _deviceRepoManager;
        IDeploymentActivityQueueManager _deploymentActivityQueueManager;

        public DeploymentInstanceManager(IDeviceRepositoryManager deviceRepoManager, IDeploymentConnectorService connector, IDeploymentHostManager hostManager, IDeviceRepositoryManager deviceManagerRepo,
                    IDeploymentActivityQueueManager deploymentActivityQueueManager, IDeploymentInstanceRepo instanceRepo, ISolutionManager solutionManager, IDeploymentHostRepo hostRepo, IDeploymentInstanceStatusRepo deploymentStatusInstanceRepo,
                    IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(hostManager, instanceRepo, deviceManagerRepo, deploymentStatusInstanceRepo, logger, appConfig, depmanager, security)
        {
            _hostManager = hostManager;
            _instanceRepo = instanceRepo;
            _solutionManager = solutionManager;
            _deploymentActivityQueueManager = deploymentActivityQueueManager;
            _connector = connector;
            _deviceRepoManager = deviceRepoManager;
            _hostRepo = hostRepo;
        }

        private async Task<InvokeResult> PerformActionAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user, DeploymentActivityTaskTypes activityType, int timeoutSeconds = 120)
        {
            var timeout = DateTime.UtcNow.Add(TimeSpan.FromSeconds(timeoutSeconds));
            
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Perform, user, org, $"{activityType}Instance");            

            await _deploymentActivityQueueManager.Enqueue(new DeploymentActivity(DeploymentActivityResourceTypes.Instance, instance.Id, activityType)
            {
                RequestedByUserId = user.Id,
                RequestedByUserName = user.Text,
                RequestedByOrganizationId = org.Id,
                RequestedByOrganizationName = org.Text,
                TimeoutTimeStamp = timeout.ToJSONString()
            });

            return InvokeResult.Success;
        }

        private InvokeResult CanTransitionToState(DeploymentHost host, DeploymentInstance instance, DeploymentActivityTaskTypes taskType, EntityHeader org, EntityHeader user)
        {

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeployHostAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.DeployHost, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.DeployHost);
        }

        public async Task<InvokeResult> StartAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Start, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Start);
        }

        public async Task<InvokeResult> PauseAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Pause, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Pause);
        }

        public async Task<InvokeResult> StopAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.Stop, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.Stop);
        }

        public async Task<InvokeResult> RestartHostAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.RestartHost, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartHost);
        }

        public async Task<InvokeResult> RestartContainerAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.RestartContainer, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.RestartContainer);
        }

        public async Task<InvokeResult> UpdateRuntimeAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.UpdateRuntime, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.UpdateRuntime);
        }

        public  async Task<InvokeResult> ReloadSolutionAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.ReloadSolution, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.ReloadSolution);
        }

        public async Task<InvokeResult> DestroyHostAsync(String id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            var host = await _hostRepo.GetDeploymentHostAsync(instance.PrimaryHost.Id);

            var transitionResult = CanTransitionToState(host, instance, DeploymentActivityTaskTypes.DestroyHost, org, user);
            if (!transitionResult.Successful) return transitionResult;

            return await PerformActionAsync(instance, org, user, DeploymentActivityTaskTypes.DestroyHost);
        }

        public async Task<InvokeResult<string>> GetRemoteMonitoringURIAsync(string channel, string id, string verbosity, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(user, org, $"wsrequest.{channel}", id);

            var notificationHost = await _hostManager.GetNotificationsHostAsync(org, user);
            return await _connector.GetRemoteMonitoringUriAsync(notificationHost, channel, id, verbosity, org, user);
        }
  

        public async Task<InvokeResult<InstanceRuntimeDetails>> GetInstanceDetailsAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await GetInstanceAsync(instanceId, org, user)
;            var host = await _hostManager.GetDeploymentHostAsync(instance.PrimaryHost.Id, org, user);
            await AuthorizeAsync(user, org, "instanceRuntimeDetails", instanceId);
            return await _connector.GetInstanceDetailsAsync(host, instanceId, org, user);
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(instance);
        }
        

        public async Task<IEnumerable<DeploymentInstanceSummary>> GetInstanceForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeploymentInstance));
            return await _instanceRepo.GetInstanceForOrgAsync(orgId);
        }

        public async Task<InvokeResult<DeploymentInstance>> LoadFullInstanceAsync(string id, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(id);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

            instance.DeviceRepository.Value = await _deviceRepoManager.GetDeviceRepositoryAsync(instance.DeviceRepository.Id, org, user);

            var solutionResult = await _solutionManager.LoadFullSolutionAsync(instance.Solution.Id, org, user);
            if (solutionResult.Successful)
            {
                instance.Solution.Value = solutionResult.Result;
                instance.Solution.Id = instance.Solution.Value.Id;
                instance.Solution.Text = instance.Solution.Value.Name;
                return InvokeResult<DeploymentInstance>.Create(instance);
            }
            else
            {
                return InvokeResult<DeploymentInstance>.FromErrors(solutionResult.Errors.ToArray());
            }
        }
        
    }
}
