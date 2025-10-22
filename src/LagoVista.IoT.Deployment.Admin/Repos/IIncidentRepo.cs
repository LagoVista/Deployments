// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c5594200d181aea8da0ddc8ae19ead36bd054239857775c5d9a912dea8d713cf
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Repos
{
    public interface IIncidentRepo
    {
        Task AddIncidentAsync(Incident incident);
        Task DeleteIncidentAsync(string id);
        Task<Incident> GetIncidentAsync(string id);
        Task<Incident> GetIncidentByKeyAsync(string key, string orgId);
        Task<ListResponse<IncidentSummary>> GetIncidentsForOrgAsync(string orgId, ListRequest listRequest);
        Task UpdateIncidentAsync(Incident incident);
    }
}
