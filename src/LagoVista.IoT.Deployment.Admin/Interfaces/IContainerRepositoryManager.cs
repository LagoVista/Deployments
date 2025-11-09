// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: edb354faa9284ecc60383a1a69681e20cf0c732701fef712cd88d0f21cdfedad
// IndexVersion: 2
// --- END CODE INDEX META ---
using System.Collections.Generic;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;

namespace LagoVista.IoT.Deployment.Admin
{
    public interface IContainerRepositoryManager
    {
        Task<InvokeResult> AddContainerRepoAsync(ContainerRepository containerRepo, EntityHeader org, EntityHeader user);
        Task<ContainerRepository> GetContainerRepoAsync(string id, EntityHeader org, EntityHeader user);
        Task<ContainerRepository> GetDefaultForRuntimeRepoAsync(EntityHeader org, EntityHeader user);
        Task<IEnumerable<ContainerRepositorySummary>> GetContainerReposForOrgAsync(string orgId, EntityHeader user);
        Task<InvokeResult> UpdateContainerRepoAsync(ContainerRepository containerRepo, EntityHeader org, EntityHeader user);
        Task<IEnumerable<DockerTag>> GetTagsFromRemoteRegistryAsync(string containerId, EntityHeader user, EntityHeader org);
    }
}