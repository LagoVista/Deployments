using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class IntegrationManager : ManagerBase, IIntegrationManager
    {
        IIntegrationRepo _repo;
        ISecureStorage _secureStorage;
        Services.IDockerRegisteryServices _dockerRegistryServices;

        public IntegrationManager(IIntegrationRepo repo, ISecureStorage secureStorage, IAdminLogger logger, Services.IDockerRegisteryServices dockerRegistryServices,
                IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security)
            : base(logger, appConfig, dependencyManager, security)
        {
            _repo = repo;
            _secureStorage = secureStorage;
            _dockerRegistryServices = dockerRegistryServices;
        }

        public async Task<InvokeResult> AddIntegrationAsync(Integration integration, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(integration, Actions.Create);
            await AuthorizeAsync(integration, AuthorizeResult.AuthorizeActions.Create, user, org);

            if (!String.IsNullOrEmpty(integration.ApiKey))
            {
                var addSecretResult = await _secureStorage.AddSecretAsync(org, integration.ApiKey);
                if (!addSecretResult.Successful) return addSecretResult.ToInvokeResult();
                integration.ApiKeySecretId = addSecretResult.Result;
                integration.ApiKey = null;
            }

            await _repo.AddIntegrationAsync(integration);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> UpdateIntegrationAsync(Integration integration, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(integration, Actions.Update);
            await AuthorizeAsync(integration, AuthorizeResult.AuthorizeActions.Update, user, org);

            if (!String.IsNullOrEmpty(integration.ApiKey))
            {
                var oldSecretId = integration.ApiKeySecretId;                

                var updateSecret = await _secureStorage.AddSecretAsync(org, integration.ApiKey);
                if (!updateSecret.Successful) return updateSecret.ToInvokeResult();
                integration.ApiKeySecretId = updateSecret.Result;
                integration.ApiKey = null;

                if(!string.IsNullOrEmpty(oldSecretId))
                {
                    await _secureStorage.RemoveSecretAsync(org, oldSecretId);
                }
            }

            await _repo.UpdateIntegrationAsync(integration);
            return InvokeResult.Success;
        }

        public async Task<Integration> GetIntegrationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var integration = await _repo.GetIntegrationAsync(id);
            await AuthorizeAsync(integration, AuthorizeResult.AuthorizeActions.Read, user, org);
            return integration;
        }

        public async Task<ListResponse<IntegrationSummary>> GetIntegrationsForOrgAsync(string orgId, ListRequest listRequest, EntityHeader user)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(TaggedContainer), Actions.Read);
            return await _repo.GetIntegrationsForOrgsAsync(orgId, listRequest);
        }
        
        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var integration = await _repo.GetIntegrationAsync(id);
            await AuthorizeAsync(integration, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(integration);
        }

        public Task<bool> QueryKeyInUse(string key, EntityHeader org)
        {
            return _repo.QueryKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> DeleteIntegrationAsync(string id, EntityHeader org, EntityHeader user)
        {
            var integration = await _repo.GetIntegrationAsync(id);
            await AuthorizeAsync(integration, AuthorizeResult.AuthorizeActions.Delete   , user, org);
            var result = await CheckForDepenenciesAsync(integration);
            if(result.IsInUse)
            {
                throw new InUseException();
            }

            await _repo.DeleteIntegrationAsync(id);

            return InvokeResult.Success;
        }
    }
}