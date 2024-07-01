using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IIncidentProtocolManager
    {
        Task<InvokeResult> AddIncidentProtocolAsync(IncidentProtocol incidentProtocol, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteIncidentProtocolAsync(string id, EntityHeader org, EntityHeader user);
        Task<IncidentProtocol> GetIncidentProtocolAsync(string id, EntityHeader org, EntityHeader user);
        Task<IncidentProtocol> GetIncidentProtocolByKeyAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<IncidentProtocolSummary>> GetIncidentProtocolsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<InvokeResult> UpdateIncidentProtocolAsync(IncidentProtocol incidentProtocol, EntityHeader org, EntityHeader user);
    }
}
