﻿using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IIntegrationManager
    {
        Task<InvokeResult> AddIntegrationAsync(Integration containerRepo, EntityHeader org, EntityHeader user);
        Task<Integration> GetIntegrationAsync(string id, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteIntegrationAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<IntegrationSummary>> GetIntegrationsForOrgAsync(string orgId, ListRequest listRequest, EntityHeader user);
        Task<InvokeResult> UpdateIntegrationAsync(Integration integration, EntityHeader org, EntityHeader user);
        Task<DependentObjectCheckResult> CheckInUseAsync(string id, EntityHeader org, EntityHeader user);
        Task<bool> QueryKeyInUse(string key, EntityHeader org);
    }
}