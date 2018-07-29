using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using static LagoVista.Core.Models.AuthorizeResult;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class ClientAppManager : ManagerBase, IClientAppManager
    {
        IClientAppRepo _repo;

        public ClientAppManager(IClientAppRepo repo, IAppConfig appConfig, IAdminLogger logger, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _repo = repo;
        }

        public async Task<InvokeResult> AddClientAppAsync(ClientApp clientApp, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(clientApp, Actions.Create);
            await AuthorizeAsync(clientApp, AuthorizeResult.AuthorizeActions.Create, user, org);
            await _repo.AddClientAppAsync(clientApp);
            return InvokeResult.Success;
        }

        public async Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user)
        {
            var clientAppp = await _repo.GetClientAppAsync(id);
            await AuthorizeAsync(clientAppp, AuthorizeResult.AuthorizeActions.Read, user, org);
            return await CheckForDepenenciesAsync(clientAppp);
        }

        public async Task<InvokeResult> DeleteClientAppAsync(string id, EntityHeader org, EntityHeader user)
        {
            var clientApp = await GetClientAppAsync(id, org, user);
            await AuthorizeAsync(clientApp, AuthorizeActions.Delete, user, org);
            await ConfirmNoDepenenciesAsync(clientApp);
            await _repo.DeleteClientAppAsync(clientApp.Id);

            return InvokeResult.Success;
        }

        public async Task<ClientApp> GetClientAppAsync(string id, EntityHeader org, EntityHeader user)
        {
            var clientApp = await _repo.GetClientAppAsync(id);
            await AuthorizeAsync(clientApp, AuthorizeActions.Read, user, org);
            return clientApp;
        }

        public async Task<ListResponse<ClientAppSummary>> GetClientAppsForOrgAsync(string orgId, EntityHeader user, ListRequest request)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Solution));
            return await _repo.GetClientAppsForOrgAsync(orgId, request);
        }

        public Task<bool> QueryKeyInUse(string key, EntityHeader org)
        {
            return _repo.QueryKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> UpdateClientAppAsync(ClientApp clientApp, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(clientApp, AuthorizeActions.Update, user, org);

            var result = Validator.Validate(clientApp, Actions.Update);
            await _repo.UpdateClientAppAsync(clientApp);
            return result.ToInvokeResult();
        }
    }
}
