using LagoVista.Core;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    public class ClientAppController : LagoVistaBaseController
    {
        IClientAppManager _clientAppManager;

        public ClientAppController(IClientAppManager clientAppManager, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _clientAppManager = clientAppManager;
        }

        /// <summary>
        /// Client Apps - Add New
        /// </summary>
        /// <param name="clientApp"></param>
        /// <returns></returns>
        [HttpPost("/api/clientapp")]
        public Task<InvokeResult> AddClientAppAsync([FromBody] ClientApp clientApp)
        {
            return _clientAppManager.AddClientAppAsync(clientApp, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client Apps - Get Client App
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/clientapp/{id}")]
        public async Task<DetailResponse<ClientApp>> GetClientAppAsync(string id)
        {
            var container = await _clientAppManager.GetClientAppAsync(id, OrgEntityHeader, UserEntityHeader);
            return DetailResponse<ClientApp>.Create(container);
        }

        /// <summary>
        /// Client Apps - Delete Client App
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/api/clientapp/{id}")]
        public Task<InvokeResult> DeleteClientApp(string id)
        {
            return _clientAppManager.DeleteClientAppAsync(id, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client Apps - Update Client App
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        [HttpPut("/api/clientapp")]
        public Task<InvokeResult> UpdateClientAppAsync([FromBody] ClientApp container)
        {
            SetUpdatedProperties(container);
            return _clientAppManager.UpdateClientAppAsync(container, OrgEntityHeader, UserEntityHeader);
        }

        /// <summary>
        /// Client Apps - Get Repos for Current Org
        /// </summary>
        /// <returns></returns>        
        [HttpGet("/api/clientapps")]
        public Task<ListResponse<ClientAppSummary>> GetClientAppsForOrgAsync()
        {
            return _clientAppManager.GetClientAppsForOrgAsync(OrgEntityHeader.Id, UserEntityHeader, GetListRequestFromHeader());
        }


        /// <summary>
        /// Client Apps - Create New
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/clientapp/factory")]
        public DetailResponse<ClientApp> CreateClientAppAsync()
        {
            var response = DetailResponse<ClientApp>.Create();
            response.Model.Id = Guid.NewGuid().ToId();

            response.Model.AppAuthKeyPrimary = _clientAppManager.GenerateAPIKey();
            response.Model.AppAuthKeySecondary = _clientAppManager.GenerateAPIKey();

            SetAuditProperties(response.Model);
            SetOwnedProperties(response.Model);

            return response;
        }

        /// <summary>
        /// Generate a new API Key to be used on client app
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/clientapp/apikey/generate")]
        public InvokeResult<string> GenerateRandomKey()
        {
            return InvokeResult<string>.Create(_clientAppManager.GenerateAPIKey());
        }

        /// <summary>
        /// Client Apps = Get API Keys
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/clientapp/{id}/secrets")]
        public Task<InvokeResult<ClientAppSecrets>> GetSecrets(String id)
        {
            return _clientAppManager.GetClientAppSecretsAsync(id, OrgEntityHeader, UserEntityHeader);
        }
    }
}