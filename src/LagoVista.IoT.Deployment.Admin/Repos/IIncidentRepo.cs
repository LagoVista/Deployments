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
