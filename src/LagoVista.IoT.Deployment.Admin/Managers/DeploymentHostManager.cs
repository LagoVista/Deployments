using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class InUseException : Exception { }

    public class DeploymentHostManager : ManagerBase, IDeploymentHostManager
    {
        IDeploymentHostRepo _deploymentHostRepo;

        public DeploymentHostManager(IDeploymentHostRepo deploymentHostRepo, ILogger logger, IAppConfig appConfig, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _deploymentHostRepo = deploymentHostRepo;
        }

        public async Task<InvokeResult> AddDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Create);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Create, user, org);            
            await _deploymentHostRepo.AddDeploymentHostAsync(host);

            return InvokeResult.Success;
        }        

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(id);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(host);
        }

        public async Task<InvokeResult> DeleteDeploymentHostAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);            
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            await ConfirmNoDepenenciesAsync(host);
            await _deploymentHostRepo.DeleteDeploymentHostAsync(instanceId);
            return InvokeResult.Success;
        }

        public async Task<DeploymentHost> GetDeploymentHostAsync(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return host;
        }

        public async Task<IEnumerable<DeploymentHostSummary>> GetDeploymentHostsForOrgAsync(string orgId,  EntityHeader user)
        {
            await AuthorizeOrgAccess(user, orgId, typeof(DeploymentHost));            
            return await _deploymentHostRepo.GetDeploymentsForOrgAsync(orgId);
        }

        public Task<DeploymentHost> LoadFullDeploymentHostAsync(string id)
        {
            return _deploymentHostRepo.GetDeploymentHostAsync(id);
        }

        public Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org)
        {
            return _deploymentHostRepo.QueryInstanceKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> RegenerateAccessKeys(string instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _deploymentHostRepo.GetDeploymentHostAsync(instanceId);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            host.GenerateAccessKeys();
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateDeploymentHostAsync(DeploymentHost host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Update);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);            
            await _deploymentHostRepo.UpdateDeploymentHostAsync(host);
            return InvokeResult.Success;
        }
    }
}
