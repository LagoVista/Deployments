using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IIncidentProtocolRepo
    {
        Task AddIncidentProtocolAsync(IncidentProtocol incidentProtocol);
        Task DeleteIncidentProtocolAsync(string id);
        Task<IncidentProtocol> GetIncidentProtocolAsync(string id);
        Task<IncidentProtocol> GetIncidentProtocolByKeyAsync(string key, string orgId);
        Task<ListResponse<IncidentProtocolSummary>> GetIncidentProtocolsForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateIncidentProtocolAsync(IncidentProtocol incidentProtocol);
    }
}
