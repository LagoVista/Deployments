using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using System;
using System.Threading.Tasks;
using static LagoVista.Core.Models.AuthorizeResult;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class ClientAppManager : ManagerBase, IClientAppManager
    {
        private readonly IClientAppRepo _repo;
        private readonly ISecureStorage _secureStorage;
        private readonly IUserManager _userManager;
        private readonly IOrganizationManager _orgManager;

        public ClientAppManager(IClientAppRepo repo, IAppConfig appConfig, IAdminLogger logger, ISecureStorage secureStorage,
            IUserManager userManager, IOrganizationManager orgManager, IDependencyManager depmanager, ISecurity security) : base(logger, appConfig, depmanager, security)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
        }

        public async Task<InvokeResult> AddClientAppAsync(ClientApp clientApp, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(clientApp, Actions.Create);

            await AuthorizeAsync(clientApp, AuthorizeResult.AuthorizeActions.Create, user, org);

            var primaryAddResult = await _secureStorage.AddSecretAsync(org, clientApp.AppAuthKeyPrimary);
            if (!primaryAddResult.Successful)
            {
                return primaryAddResult.ToInvokeResult();
            }

            var secondaryAddResult = await _secureStorage.AddSecretAsync(org, clientApp.AppAuthKeySecondary);
            if (!secondaryAddResult.Successful)
            {
                return secondaryAddResult.ToInvokeResult();
            }

            clientApp.AppAuthKeyPrimarySecureId = primaryAddResult.Result;
            clientApp.AppAuthKeyPrimary = null;

            clientApp.AppAuthKeySecondarySecureId = secondaryAddResult.Result;
            clientApp.AppAuthKeySecondary = null;

            var clientAppUserId = Guid.NewGuid().ToId();
            clientApp.ClientAppUser = EntityHeader.Create(clientAppUserId, $"{clientApp.Key} Service Account");

            var fullOrg = await _orgManager.GetOrganizationAsync(org.Id, org, user);

            var clientAppEmail = $"{fullOrg.Namespace}.{clientApp.Key}@nodomain.cantlogin";

            var result = await _userManager.CreateAsync(new UserAdmin.Models.Users.AppUser()
            {
                CurrentOrganization = org,
                Email = clientAppEmail,
                FirstName = clientApp.Name,
                LastName = "Service Account",
                Id = clientAppUserId,
                UserName = clientAppEmail,
                OwnerOrganization = org,
                IsAppBuilder = true,
                IsOrgAdmin = false,
                IsSystemAdmin = false,
                IsRuntimeuser = true,
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                CreationDate = clientApp.CreationDate,
                LastUpdatedDate = clientApp.CreationDate,
                LastUpdatedBy = user,
                CreatedBy = user,
                IsAccountDisabled = false,
                Name = clientApp.ClientAppUser.Text,
                PhoneNumber = "612 555-1212",
            }, $"NuvI0Tabc{Guid.NewGuid().ToId()}");


            if(!result.Successful)
            {
                return result;
            }

            await _orgManager.AddUserToOrgAsync(org.Id, clientAppUserId, org, user);
            await _repo.AddClientAppAsync(clientApp);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult<ClientApp>> AuthorizeAppAsync(string clientAppId, string apiKey)
        {
            if (String.IsNullOrEmpty(clientAppId))
            {
                return InvokeResult<ClientApp>.FromError("Missing Client App Id.");
            }

            if (String.IsNullOrEmpty(apiKey))
            {
                return InvokeResult<ClientApp>.FromError("Missing Api Key.");
            }

            var clientApp = await _repo.GetClientAppAsync(clientAppId);
            if (clientApp == null)
            {
                return InvokeResult<ClientApp>.FromError("Invalid Client App Id");
            }

            var secret1 = await _secureStorage.GetSecretAsync(clientApp.OwnerOrganization, clientApp.AppAuthKeyPrimarySecureId, clientApp.ClientAppUser);
            if (!secret1.Successful)
            {
                return InvokeResult<ClientApp>.FromInvokeResult(secret1.ToInvokeResult());
            }

            if (apiKey == secret1.Result)
            {
                return InvokeResult<ClientApp>.Create(clientApp);
            }

            var secret2 = await _secureStorage.GetSecretAsync(clientApp.OwnerOrganization, clientApp.AppAuthKeySecondarySecureId, clientApp.ClientAppUser);
            if (!secret2.Successful)
            {
                return InvokeResult<ClientApp>.FromInvokeResult(secret2.ToInvokeResult());
            }

            if (apiKey == secret2.Result)
            {
                return InvokeResult<ClientApp>.Create(clientApp);
            }

            return InvokeResult<ClientApp>.FromError("Invalid API Key");
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

        public async Task<InvokeResult<ClientAppSecrets>> GetClientAppSecretsAsync(string id, EntityHeader org, EntityHeader user)
        {
            var clientApp = await _repo.GetClientAppAsync(id);
            await AuthorizeAsync(clientApp, AuthorizeActions.Read, user, org, "GetSecrets");

            var secret1 = await _secureStorage.GetSecretAsync(org, clientApp.AppAuthKeyPrimarySecureId, user);
            if (!secret1.Successful)
            {
                return InvokeResult<ClientAppSecrets>.FromInvokeResult(secret1.ToInvokeResult());
            }

            var secret2 = await _secureStorage.GetSecretAsync(org, clientApp.AppAuthKeySecondarySecureId, user);
            if (!secret2.Successful)
            {
                return InvokeResult<ClientAppSecrets>.FromInvokeResult(secret2.ToInvokeResult());
            }

            return new InvokeResult<ClientAppSecrets>()
            {
                Result = new ClientAppSecrets()
                {
                    AppAuthKeyPrimary = secret1.Result,
                    AppAuthKeySecondary = secret2.Result
                }
            };
        }

        public async Task<ClientApp> GetClientAppAsync(string id, EntityHeader org, EntityHeader user)
        {
            var clientApp = await _repo.GetClientAppAsync(id);
            await AuthorizeAsync(clientApp, AuthorizeActions.Read, user, org);
            return clientApp;
        }

        public async Task<KioskClientAppSummary> GetKioskClientAppAsync(string orgId, string kioskId, EntityHeader org, EntityHeader user)
        {
            var clientApp = await _repo.GetKioskClientAppAsync(orgId, kioskId);
            await AuthorizeAsync(clientApp, AuthorizeActions.Read, user, org);
            var secrets = await GetClientAppSecretsAsync(clientApp.Id, org, user);
            var summary = new KioskClientAppSummary() 
            { 
                ClientAppId = clientApp.Id,
                AppKey = secrets.Result.AppAuthKeyPrimary
            };
            return summary;
        }

        public async Task<ListResponse<ClientAppSummary>> GetClientAppsForOrgAsync(string orgId, EntityHeader user, ListRequest request)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Solution));
            return await _repo.GetClientAppsForOrgAsync(orgId, request);
        }

        private static Random _rnd = new Random();

        public string GenerateAPIKey()
        {
            var len = 64;

            var buffer = new byte[len];
            _rnd.NextBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public Task<bool> QueryKeyInUse(string key, EntityHeader org)
        {
            return _repo.QueryKeyInUseAsync(key, org.Id);
        }

        public async Task<InvokeResult> UpdateClientAppAsync(ClientApp clientApp, EntityHeader org, EntityHeader user)
        {
            await AuthorizeAsync(clientApp, AuthorizeActions.Update, user, org);

            var result = Validator.Validate(clientApp, Actions.Update);

            if (!string.IsNullOrEmpty(clientApp.AppAuthKeyPrimary))
            {
                var removeResult = await _secureStorage.RemoveSecretAsync(org, clientApp.AppAuthKeyPrimarySecureId);
                if (!removeResult.Successful)
                {
                    return removeResult.ToInvokeResult();
                }

                var primaryAddResult = await _secureStorage.AddSecretAsync(org, clientApp.AppAuthKeyPrimary);
                if (!primaryAddResult.Successful)
                {
                    return primaryAddResult.ToInvokeResult();
                }

                clientApp.AppAuthKeyPrimarySecureId = primaryAddResult.Result;
                clientApp.AppAuthKeyPrimary = null;
            }

            if (!string.IsNullOrEmpty(clientApp.AppAuthKeySecondary))
            {
                var removeResult = await _secureStorage.RemoveSecretAsync(org, clientApp.AppAuthKeySecondarySecureId);
                if (!removeResult.Successful)
                {
                    return removeResult.ToInvokeResult();
                }

                var secondaryAddResult = await _secureStorage.AddSecretAsync(org, clientApp.AppAuthKeySecondary);
                if (!secondaryAddResult.Successful)
                {
                    return secondaryAddResult.ToInvokeResult();
                }

                clientApp.AppAuthKeySecondarySecureId = secondaryAddResult.Result;
                clientApp.AppAuthKeySecondary = null;
            }

            await _repo.UpdateClientAppAsync(clientApp);

            return result.ToInvokeResult();
        }
    }
}
