// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6fc3d6fe5c0b8f910c4b816e69684a4483a8fc417353ddb834636fe4edec9d03
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IContainerRepositoryRepo
    {
        Task AddContainerRepoAsync(ContainerRepository containerRepository);
        Task<ContainerRepository> GetContainerRepoAsync(string containerRepositoryId);
        Task<ContainerRepository> GetDefaultForRuntimeAsync();
        Task UpdateContainerRepoAsync(ContainerRepository containerRepositry);
        Task<IEnumerable<ContainerRepositorySummary>> GetContainerReposForOrgAsync(string orgId);
    }
}
