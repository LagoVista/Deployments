// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9eece90a564bc71d76a6d87f13f05453c5efe3a7a24fabac0f07be19268c7bc7
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.CloudStorage.Interfaces;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class IncidentRepo : DocumentDBRepoBase<Incident>, IIncidentRepo
    {
        private bool _shouldConsolidateCollections;

        public IncidentRepo(IDeploymentRepoSettings repoSettings, IDocumentCloudCachedServices services)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, services)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }
        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddIncidentAsync(Incident incident)
        {
            return CreateDocumentAsync(incident);
        }

        public Task DeleteIncidentAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<Incident> GetIncidentAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<Incident> GetIncidentByKeyAsync(string key, string orgId)
        {
            var result = await QueryAsync(erc => erc.Key == key && erc.OwnerOrganization.Id == orgId);
            return result.FirstOrDefault();
        }

        public Task<ListResponse<IncidentSummary>> GetIncidentsForOrgAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<IncidentSummary, Incident>(ec => ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
        }

        public Task UpdateIncidentAsync(Incident incident)
        {
            return UpsertDocumentAsync(incident);
        }
    }
}
