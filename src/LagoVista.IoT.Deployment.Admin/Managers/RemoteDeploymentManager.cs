// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f790843b6719e57e8d05116b5305da57e7c38ab2df674fff13b0661cf5ea7088
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.Core;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class RemoteDeploymentManager : ManagerBase, IRemoteDeploymentManager
    {
        IRemoteDeploymentRepo _repo;

        public RemoteDeploymentManager(IRemoteDeploymentRepo repo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) : 
            base(logger, appConfig, dependencyManager, security)
        {
            _repo = repo;
        }

        public async Task<InvokeResult> AddRemoteDeploymentAsync(RemoteDeployment host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Create);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Create, user, org);
            await _repo.AddRemoteDeploymentAsync(host);

            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var host = await _repo.GetRemoteDeploymentAsync(id);
            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(host);
        }

        public async Task<InvokeResult> DeleteRemoteDeploymentHostAsync(String instanceId, EntityHeader org, EntityHeader user)
        {
            var host = await _repo.GetRemoteDeploymentAsync(instanceId);

            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            await ConfirmNoDepenenciesAsync(host);
            await _repo.DeleteRemoteDeploymentAsync(instanceId);
            return InvokeResult.Success;
        }


        public async Task<RemoteDeployment> GetRemoteDeploymentAsync(string id, EntityHeader org, EntityHeader user)
        {
            var host = await _repo.GetRemoteDeploymentAsync(id);
            if (host == null) return host;

            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Read, user, org);
            return host;
        }

        public async Task<ListResponse<RemoteDeploymentSummary>> GetRemoteDeploymentsForOrgAsync(string orgId, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(DeploymentHost));
            return await _repo.GetRemoteDeploymentsForOrgsAsync(orgId);
        }

        public Task<RemoteDeployment> LoadFullRemoteDeploymentAsync(string id, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> QueryDeploymentHostKeyInUseAsync(string key, EntityHeader org)
        {
            return _repo.QueryKeyInUseAsync(key, org.Id);
        }

        public Task<InvokeResult> RegenerateAccessKeys(string id, EntityHeader org, EntityHeader user)
        {
            throw new NotImplementedException();
        }

        public async Task<InvokeResult> UpdateRemoteDeploymentAsync(RemoteDeployment host, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(host, Actions.Update);

            await AuthorizeAsync(host, AuthorizeResult.AuthorizeActions.Update, user, org);
            host.LastUpdatedDate = DateTime.UtcNow.ToJSONString();
            host.LastUpdatedBy = user;
            await _repo.UpdateRemoteeDeploymentAsync(host);
            return InvokeResult.Success;

        }
    }
}
