using LagoVista.Core.Interfaces;
using LagoVista.Core;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.Core.Exceptions;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentInstanceManagerCore : ManagerBase, IDeploymentInstanceManagerCore
    {
        IDeploymentInstanceRepo _instanceRepo;
        IDeploymentHostManager _deploymentHostManager;
        IDeviceRepositoryManager _deviceManagerRepo;
        IDeploymentInstanceStatusRepo _deploymentInstanceStatusRepo;

        public DeploymentInstanceManagerCore(IDeploymentHostManager deploymentHostManager, IDeploymentInstanceRepo instanceRepo, IDeviceRepositoryManager deviceManagerRepo,
           IDeploymentInstanceStatusRepo deploymentInstanceStatusRepo,  IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) :
            base(logger, appConfig, depmanager, security)
        {
            _deviceManagerRepo = deviceManagerRepo;
            _instanceRepo = instanceRepo;
            _deploymentHostManager = deploymentHostManager;
            _deploymentInstanceStatusRepo = deploymentInstanceStatusRepo;
        }

        public async Task<InvokeResult> DeleteInstanceAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Delete, user, org);
            await CheckForDepenenciesAsync(instance);

            var repo = await _deviceManagerRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id, org, user);
            if(repo != null)
            {
                /* Upon deletion release the repo so it can be used somewhere else */
                repo.Instance = null;
                await _deviceManagerRepo.UpdateDeviceRepositoryAsync(repo, org, user);
            }

            //TODO: Should probably also clear the Host

            await _instanceRepo.DeleteInstanceAsync(instanceId);

            return InvokeResult.Success; ;
        }


        private DeploymentHost CreateHost(DeploymentInstance instance)
        {
            var host = new DeploymentHost
            {
                Id = Guid.NewGuid().ToId(),
                HostType = EntityHeader<HostTypes>.Create(HostTypes.Dedicated),
                CreatedBy = instance.CreatedBy,
                Name = $"{instance.Name} Host",
                LastUpdatedBy = instance.LastUpdatedBy,
                OwnerOrganization = instance.OwnerOrganization,
                OwnerUser = instance.OwnerUser,
                CreationDate = instance.CreationDate,
                LastUpdatedDate = instance.LastUpdatedDate,
                DedicatedInstance = new EntityHeader() { Id = instance.Id, Text = instance.Name },
                Key = instance.Key,
                IsPublic = false,
                Status = EntityHeader<HostStatus>.Create(HostStatus.Offline),
                Size = instance.Size,
                CloudProvider = instance.CloudProvider,
                Subscription = instance.Subscription,
                ContainerRepository = instance.ContainerRepository,
                ContainerTag = instance.ContainerTag,
                DnsHostName = instance.DnsHostName
            };

            return host;
        }

        public async Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user)
        {
            var host = CreateHost(instance);
            instance.PrimaryHost = new EntityHeader<DeploymentHost>() { Id = host.Id, Text = host.Name };

            ValidationCheck(host, Actions.Create);
            ValidationCheck(instance, Actions.Create);

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Create, user, org);

            if(!EntityHeader.IsNullOrEmpty(instance.DeviceRepository))
            {
                var repo = await _deviceManagerRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id, org, user);
                if(EntityHeader.IsNullOrEmpty(repo.Instance))
                {
                    repo.Instance = EntityHeader.Create(instance.Id, instance.Name);
                    await _deviceManagerRepo.UpdateDeviceRepositoryAsync(repo, org, user);
                }
                else
                {
                    throw new ValidationException("Can not reassign repo", new ErrorMessage($"Repository is already assigned to the instance {repo.Instance.Text}.  A Device Repository can only be assigned to one Instance at a time."));
                }
            }

            await _instanceRepo.AddInstanceAsync(instance);
            await _deploymentHostManager.AddDeploymentHostAsync(host, org, user);

            return InvokeResult.Success;
        }


        public Task<bool> QueryInstanceKeyInUseAsync(string key, EntityHeader org)
        {
            return _instanceRepo.QueryInstanceKeyInUseAsync(key, org.Id);
        }

        public async Task<DeploymentInstance> GetInstanceAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Read, user, org);

            return instance;
        }

        public async Task<InvokeResult> UpdateInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(instance, Actions.Update);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Update, user, org);

            var existingInstance = await _instanceRepo.GetInstanceAsync(instance.Id);
            if(existingInstance.DeviceRepository.Id != instance.DeviceRepository.Id)
            {
                var newlyAssginedRepo = await _deviceManagerRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id, org, user);
                if(!EntityHeader.IsNullOrEmpty(newlyAssginedRepo.Instance))
                {
                    throw new ValidationException("Device Repository In Use", new ErrorMessage($"Repository is already assigned to the instance {newlyAssginedRepo.Instance.Text}.  A Device Repository can only be assigned to one Instance at a time."));
                }

                newlyAssginedRepo.Instance = EntityHeader.Create(instance.Id, instance.Name);
                await _deviceManagerRepo.UpdateDeviceRepositoryAsync(newlyAssginedRepo, org, user);

                var oldRepo = await _deviceManagerRepo.GetDeviceRepositoryAsync(existingInstance.DeviceRepository.Id, org, user);
                oldRepo.Instance = null;
                await _deviceManagerRepo.UpdateDeviceRepositoryAsync(oldRepo, org, user);
            }

            if(existingInstance.Status.Value != instance.Status.Value ||
               existingInstance.IsDeployed != instance.IsDeployed)
            {
                await UpdateInstanceStatusAsync(instance.Id, instance.Status.Value, instance.IsDeployed, org, user);
            }

            var solution = instance.Solution.Value;
            instance.Solution.Value = null;
            await _instanceRepo.UpdateInstanceAsync(instance);
            instance.Solution.Value = solution;

            if (!EntityHeader.IsNullOrEmpty(instance.PrimaryHost))
            {
                var host = await _deploymentHostManager.GetDeploymentHostAsync(instance.PrimaryHost.Id, org, user);
                if (host.Size.Id != instance.Size.Id ||
                   host.CloudProvider.Id != instance.CloudProvider.Id ||
                   host.Subscription.Id != instance.Subscription.Id ||
                   host.ContainerRepository.Id != instance.ContainerRepository.Id ||
                   host.ContainerTag.Id != instance.ContainerTag.Id)
                {
                    host.Size = instance.Size;
                    host.Subscription = instance.Subscription;
                    host.CloudProvider = instance.CloudProvider;
                    host.ContainerRepository = instance.ContainerRepository;
                    host.ContainerTag = instance.ContainerTag;
                    await _deploymentHostManager.UpdateDeploymentHostAsync(host, org, user);
                }
            }

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateInstanceStatusAsync(string instanceId, DeploymentInstanceStates newStatus, bool deployed, EntityHeader org, EntityHeader user, string details = "")
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            ValidationCheck(instance, Actions.Update);

            var statusUpdate = Models.DeploymentInstanceStatus.Create(instanceId, user);
            statusUpdate.Details = details;
            statusUpdate.OldDeploy = instance.IsDeployed;
            statusUpdate.NewDeploy = deployed;
            statusUpdate.OldState = instance.Status.Value.ToString();
            statusUpdate.NewState = newStatus.ToString();
            await _deploymentInstanceStatusRepo.AddDeploymentInstanceStatusAsync(statusUpdate);

            instance.Status = EntityHeader<DeploymentInstanceStates>.Create(newStatus);
            instance.IsDeployed = deployed;
            await _instanceRepo.UpdateInstanceAsync(instance);
            return InvokeResult.Success;
        }
    }
}
