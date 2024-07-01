using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepos.Repos
{
    public class IncidentProtocolRepo : DocumentDBRepoBase<IncidentProtocol>, IIncidentProtocolRepo
    {
        private bool _shouldConsolidateCollections;

        public IncidentProtocolRepo(IDeploymentRepoSettings repoSettings, IAdminLogger logger, ICacheProvider cacheProvider)
            : base(repoSettings.DeploymentAdminDocDbStorage.Uri, repoSettings.DeploymentAdminDocDbStorage.AccessKey, repoSettings.DeploymentAdminDocDbStorage.ResourceName, logger, cacheProvider)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }
        protected override bool ShouldConsolidateCollections => _shouldConsolidateCollections;

        public Task AddIncidentProtocolAsync(IncidentProtocol incidentProtocol)
        {
            return CreateDocumentAsync(incidentProtocol);
        }

        public Task DeleteIncidentProtocolAsync(string id)
        {
            return DeleteDocumentAsync(id);
        }

        public Task<IncidentProtocol> GetIncidentProtocolAsync(string id)
        {
            return GetDocumentAsync(id);
        }

        public async Task<IncidentProtocol> GetIncidentProtocolByKeyAsync(string key, string orgId)
        {
            var result = await QueryAsync(erc => erc.Key == key && erc.OwnerOrganization.Id == orgId);
            return result.FirstOrDefault();
        }

        public Task<ListResponse<IncidentProtocolSummary>> GetIncidentProtocolsForOrgAsync(string orgId, ListRequest listRequest)
        {
            return QuerySummaryAsync<IncidentProtocolSummary, IncidentProtocol>(ec => ec.OwnerOrganization.Id == orgId, ec => ec.Name, listRequest);
        }

        public Task UpdateIncidentProtocolAsync(IncidentProtocol incidentProtocol)
        {
            return UpsertDocumentAsync(incidentProtocol);
        }
    }
}
