// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ffd87dbde61b87f4751c220ba557b4177d38b55d570fe7dc0e5a5a8506f63d81
// IndexVersion: 0
// --- END CODE INDEX META ---
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
