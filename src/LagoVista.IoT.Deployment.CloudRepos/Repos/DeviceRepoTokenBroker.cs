using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceRepoTokenBroker : IDeviceRepoTokenBroker
    {
        IDeploymentInstanceRepoSettings _repoSettings;
        IAdminLogger _logger;
        IDeploymentRepoSettings _deploymentRepo;
        IDeviceRepositoryRepo _deviceRepoRepo;
        IDeploymentInstanceRepo _instanceRepo;
        ISecureStorage _secureStorage;

        const int TOKEN_EXPIRES = 60 * 60 * 5; /* Five Hours */

        public DeviceRepoTokenBroker(IDeploymentInstanceRepoSettings repoSettings, IAdminLogger logger, IDeploymentRepoSettings deploymentRepo,
          ISecureStorage secureStorage, IDeviceRepositoryRepo deviceRepoRepo, IDeploymentInstanceRepo instanceRepo)
        {
            _repoSettings = repoSettings;
            _logger = logger;
            _instanceRepo = instanceRepo;
            _deviceRepoRepo = deviceRepoRepo;
            _deploymentRepo = deploymentRepo;
            _secureStorage = secureStorage;
        }

        private async Task CreateUserIfNotExistAsync(DocumentClient client, string databaseId, string userId)
        {
            try
            {
                await client.ReadUserAsync(UriFactory.CreateUserUri(databaseId, userId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var response = await client.CreateUserAsync(UriFactory.CreateDatabaseUri(databaseId), new User { Id = userId });
                }
            }
        }

        public async Task<InvokeResult<DeviceRepoToken>> GetDeviceRepoTokenAsync(EntityHeader instanceEh, EntityHeader org)
        {
            var instance = await _instanceRepo.GetInstanceAsync(instanceEh.Id);
            if (instance.OwnerOrganization.Id != org.Id)
            {
                throw new UnauthorizedAccessException("Instance Org Mismatch");
            }

            if (EntityHeader.IsNullOrEmpty(instance.DeviceRepository))
            {
                throw new InvalidOperationException("Attempt to create key for non NuvIoT Device Repo.");
            }

            var repo = await _deviceRepoRepo.GetDeviceRepositoryAsync(instance.DeviceRepository.Id);
            if (repo.RepositoryType.Value != DeviceManagement.Core.Models.RepositoryTypes.NuvIoT)
            {
                throw new InvalidOperationException("Attempt to create key for non NuvIoT Device Repo.");
            }

            var keyResult = await _secureStorage.GetSecretAsync(org, repo.DeviceStorageSecureSettingsId, instanceEh);
            if (!keyResult.Successful)
            {
                throw new Exception(keyResult.Errors.First().Message);
            }

            var settings = JsonConvert.DeserializeObject<ConnectionSettings>(keyResult.Result);

            var client = new DocumentClient(new System.Uri(settings.Uri), settings.AccessKey);
            await CreateUserIfNotExistAsync(client, settings.ResourceName, repo.Id);

            var permissionId = Guid.NewGuid().ToId();
            var deviceRepoId = repo.Id;
            var databaseId = settings.ResourceName;
            var collectionId = "Devices";

            DocumentCollection collection = await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));

            try
            {
                var permissionsUri = UriFactory.CreatePermissionUri(databaseId, deviceRepoId, deviceRepoId);
                Permission permission = await client.ReadPermissionAsync(permissionsUri, new RequestOptions()
                {
                    ResourceTokenExpirySeconds = TOKEN_EXPIRES
                });


                var token = new DeviceRepoToken()
                {
                    Token = permission.Token,
                    Expires = DateTime.UtcNow.AddSeconds(TOKEN_EXPIRES).ToJSONString(),
                    DeviceRepo = EntityHeader.Create(repo.Id, repo.Name),
                    Id = permissionId
                };

                return InvokeResult<DeviceRepoToken>.Create(token);
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    var newPermissionRequest = new Permission
                    {
                        PermissionMode = PermissionMode.All,
                        ResourceLink = collection.SelfLink,
                        ResourcePartitionKey = new PartitionKey(deviceRepoId),
                        Id = deviceRepoId 
                    };

                    try
                    {
                        Permission newPermission = await client.CreatePermissionAsync(UriFactory.CreateUserUri(databaseId, deviceRepoId), newPermissionRequest, new RequestOptions()
                        {
                            ResourceTokenExpirySeconds = TOKEN_EXPIRES
                        });

                        var expires = DateTime.UtcNow.AddSeconds(TOKEN_EXPIRES);

                        var token = new DeviceRepoToken()
                        {
                            Token = newPermission.Token,
                            Expires = expires.ToJSONString(),
                            DeviceRepo = EntityHeader.Create(repo.Id, repo.Name),
                            Id = permissionId
                        };

                        return InvokeResult<DeviceRepoToken>.Create(token);
                    }
                    catch(Exception createException)
                    {
                        _logger.AddException("DeviceRepoTokenBroker_GetDeviceRepoTokenAsync_Create", createException, instance.Id.ToKVP("instanceId"), org.Id.ToKVP("orgId"));
                        return InvokeResult<DeviceRepoToken>.FromError(createException.Message);
                    }
                }
                else
                {
                    _logger.AddException("DeviceRepoTokenBroker_GetDeviceRepoTokenAsync_Get", ex, instance.Id.ToKVP("instanceId"), org.Id.ToKVP("orgId"));
                    return InvokeResult<DeviceRepoToken>.FromError(ex.Message);
                }
            }
        }
    }
}
