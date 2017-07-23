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
        Task UpdateContainerRepoAsync(ContainerRepository containerRepositry);
        Task<IEnumerable<ContainerRepositorySummary>> GetContainerReposForOrgAsync(string orgId);
    }
}
