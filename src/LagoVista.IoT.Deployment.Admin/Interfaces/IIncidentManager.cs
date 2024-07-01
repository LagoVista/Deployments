using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IIncidentManager
    {
        Task<InvokeResult> AddIncidentAsync(Incident incident, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteIncidentAsync(string id, EntityHeader org, EntityHeader user);
        Task<Incident> GetIncidentAsync(string id, EntityHeader org, EntityHeader user);
        Task<Incident> GetIncidentByKeyAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<IncidentSummary>> GetIncidentsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateIncidentAsync(Incident incident, EntityHeader org, EntityHeader user);
    }
}
