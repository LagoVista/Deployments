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

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class DeploymentInstanceManagerCore : ManagerBase, IDeploymentInstanceManagerCore
    {
        IDeploymentInstanceRepo _instanceRepo;
        IDeploymentHostManager _deploymentHostManager;

        public DeploymentInstanceManagerCore(IDeploymentHostManager deploymentHostManager, IDeploymentInstanceRepo instanceRepo, IAdminLogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) :
            base(logger, appConfig, depmanager, security)
        {
            _instanceRepo = instanceRepo;
            _deploymentHostManager = deploymentHostManager;
        }

        public async Task<InvokeResult> DeleteInstanceAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceId);
            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Delete, user, org);
            await CheckForDepenenciesAsync(instance);

            await _instanceRepo.DeleteInstanceAsync(instanceId);

            return InvokeResult.Success; ;
        }


        private DeploymentHost CreateHost(DeploymentInstance instance)
        {
            var host = new DeploymentHost();
            host.Id = Guid.NewGuid().ToId();
            host.HostType = EntityHeader<HostTypes>.Create(HostTypes.Dedicated);
            host.CreatedBy = instance.CreatedBy;
            host.Name = $"{instance.Name} Host";
            host.LastUpdatedBy = instance.LastUpdatedBy;
            host.OwnerOrganization = instance.OwnerOrganization;
            host.OwnerUser = instance.OwnerUser;
            host.CreationDate = instance.CreationDate;
            host.LastUpdatedDate = instance.LastUpdatedDate;
            host.DedicatedInstance = new EntityHeader() { Id = instance.Id, Text = instance.Name };
            host.Key = instance.Key;
            host.IsPublic = false;
            host.Status = EntityHeader<HostStatus>.Create(HostStatus.Offline);
            host.Size = instance.Size;
            host.CloudProvider = instance.CloudProvider;
            host.Subscription = instance.Subscription;
            host.ContainerRepository = instance.ContainerRepository;
            host.ContainerTag = instance.ContainerTag;
            host.DnsHostName = instance.DnsHostName;

            return host;
        }

        public async Task<InvokeResult> AddInstanceAsync(DeploymentInstance instance, EntityHeader org, EntityHeader user)
        {
            var host = CreateHost(instance);
            instance.Host = new EntityHeader<DeploymentHost>() { Id = host.Id, Text = host.Name };

            ValidationCheck(host, Actions.Create);
            ValidationCheck(instance, Actions.Create);

            await AuthorizeAsync(instance, AuthorizeResult.AuthorizeActions.Create, user, org);
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

            var solution = instance.Solution.Value;
            instance.Solution = null;

            await _instanceRepo.UpdateInstanceAsync(instance);

            instance.Solution.Value = solution;

            return InvokeResult.Success;
        }
    }
}
