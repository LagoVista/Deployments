// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 11c3fb1799a4c508bebc2d914456f405f38aa525d0076d6145857a81dcf323e7
// IndexVersion: 0
// --- END CODE INDEX META ---
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
