using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using System;
using System.Threading.Tasks;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Deployment.Admin.Repos;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    public class IncidentProtocolManager : ManagerBase, IIncidentProtocolManager
    {
        private readonly IIncidentProtocolRepo _incidentProtocolsRepo;

        public IncidentProtocolManager(IIncidentProtocolRepo incidentProtocolsRepo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) :
            base(logger, appConfig, dependencyManager, security)
        {
            _incidentProtocolsRepo = incidentProtocolsRepo ?? throw new ArgumentNullException(nameof(incidentProtocolsRepo));
        }

        public async Task<InvokeResult> AddIncidentProtocolAsync(IncidentProtocol incidentProtocol, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(incidentProtocol, Actions.Create);
            await AuthorizeAsync(incidentProtocol, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _incidentProtocolsRepo.AddIncidentProtocolAsync(incidentProtocol);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteIncidentProtocolAsync(string id, EntityHeader org, EntityHeader user)
        {
            var incidentProtocol = await _incidentProtocolsRepo.GetIncidentProtocolAsync(id);
            await ConfirmNoDepenenciesAsync(incidentProtocol);

            await AuthorizeAsync(incidentProtocol, AuthorizeResult.AuthorizeActions.Delete, user, org);

            await _incidentProtocolsRepo.DeleteIncidentProtocolAsync(id);

            return InvokeResult.Success;
        }

        public async Task<IncidentProtocol> GetIncidentProtocolAsync(string id, EntityHeader org, EntityHeader user)
        {
            var incidentProtocol = await _incidentProtocolsRepo.GetIncidentProtocolAsync(id);
            await AuthorizeAsync(incidentProtocol, AuthorizeResult.AuthorizeActions.Read, user, org);

            return incidentProtocol;
        }

        public async Task<IncidentProtocol> GetIncidentProtocolByKeyAsync(string key, EntityHeader org, EntityHeader user)
        {
            var incidentProtocol = await _incidentProtocolsRepo.GetIncidentProtocolByKeyAsync(key, org.Id);
            await AuthorizeAsync(incidentProtocol, AuthorizeResult.AuthorizeActions.Read, user, org);

            return incidentProtocol;
        }

        public async Task<ListResponse<IncidentProtocolSummary>> GetIncidentProtocolsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(IncidentProtocol));
            return await _incidentProtocolsRepo.GetIncidentProtocolsForOrgAsync(orgId, listRequest);
        }

        public async Task<InvokeResult> UpdateIncidentProtocolAsync(IncidentProtocol incidentProtocol, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(incidentProtocol, Actions.Create);
            await AuthorizeAsync(incidentProtocol, AuthorizeResult.AuthorizeActions.Update, user, org);

            await _incidentProtocolsRepo.UpdateIncidentProtocolAsync(incidentProtocol);

            return InvokeResult.Success;
        }
    }
}
