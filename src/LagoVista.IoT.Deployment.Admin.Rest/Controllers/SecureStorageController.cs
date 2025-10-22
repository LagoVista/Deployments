// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 24327f4283c58b973b2b20ed4d798a237952725520ce8220bd9fa65dd16c311b
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Controllers;
using LagoVista.UserAdmin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
    [Authorize]
    public class SecureStorageController : LagoVistaBaseController
    {
        private readonly ISecureStorage _secureStorage;

        public SecureStorageController(ISecureStorage secureStorage, UserManager<AppUser> userManager, IAdminLogger logger) : base(userManager, logger)
        {
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        }

        /// <summary>
        /// Container Repo - Get Container
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/secret/{id}")]
        public async Task<InvokeResult<string>> GetContainerAsync(string id)
        {
            return await _secureStorage.GetSecretAsync(OrgEntityHeader, id, UserEntityHeader);
        }
    }
}
