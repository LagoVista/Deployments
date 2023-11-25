﻿using LagoVista.CloudStorage;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class DeviceErrorCodesRepo : DocumentDBRepoBase<DeviceErrorCode>, IDeviceErrorCodesRepo
    {
        private bool _shouldConsolidateCollections;

        public DeviceErrorCodesRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }
        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddErrorCodeAsync(DeviceErrorCode errorCode)
        {
            return CreateDocumentAsync(errorCode);
        }

        public Task DeleteErrorCodeAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<DeviceErrorCode> GetErrorCodeAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<ListResponse<DeviceErrorCodeSummary>> GetErrorCodesForOrgAsync(string orgId, ListRequest listRequest)
        {
            var errorCodes = await QueryAsync(ec => ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
            return ListResponse<DeviceErrorCodeSummary>.Create(errorCodes.Model.Select(ec => ec.CreateSummary()), errorCodes);
        }

        public Task UpdateErrorCodeAsync(DeviceErrorCode errorCode)
        {
            return UpsertDocumentAsync(errorCode);
        }
    }
}