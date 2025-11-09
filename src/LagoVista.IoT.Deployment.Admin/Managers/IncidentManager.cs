// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ad0b135ab6ea24b1ebc6a17dc166a789c377e625a58eeb44c4b0ac93d3f75c9d
// IndexVersion: 2
// --- END CODE INDEX META ---
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
    public class IncidentManager : ManagerBase, IIncidentManager
    {
        private readonly IIncidentRepo _incidentsRepo;

        public IncidentManager(IIncidentRepo incidentsRepo, ILogger logger, IAppConfig appConfig, IDependencyManager dependencyManager, ISecurity security) :
            base(logger, appConfig, dependencyManager, security)
        {
            _incidentsRepo = incidentsRepo ?? throw new ArgumentNullException(nameof(incidentsRepo));
        }

        public async Task<InvokeResult> AddIncidentAsync(Incident incident, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(incident, Actions.Create);
            await AuthorizeAsync(incident, AuthorizeResult.AuthorizeActions.Create, user, org);

            await _incidentsRepo.AddIncidentAsync(incident);

            return InvokeResult.Success;
        }

        public async Task<InvokeResult> DeleteIncidentAsync(string id, EntityHeader org, EntityHeader user)
        {
            var incident = await _incidentsRepo.GetIncidentAsync(id);
            await ConfirmNoDepenenciesAsync(incident);

            await AuthorizeAsync(incident, AuthorizeResult.AuthorizeActions.Delete, user, org);

            await _incidentsRepo.DeleteIncidentAsync(id);

            return InvokeResult.Success;
        }

        public async Task<Incident> GetIncidentAsync(string id, EntityHeader org, EntityHeader user)
        {
            var incident = await _incidentsRepo.GetIncidentAsync(id);
            await AuthorizeAsync(incident, AuthorizeResult.AuthorizeActions.Read, user, org);

            return incident;
        }

        public async Task<Incident> GetIncidentByKeyAsync(string key, EntityHeader org, EntityHeader user)
        {
            var incident = await _incidentsRepo.GetIncidentByKeyAsync(key, org.Id);
            await AuthorizeAsync(incident, AuthorizeResult.AuthorizeActions.Read, user, org);

            return incident;
        }

        public async Task<ListResponse<IncidentSummary>> GetIncidentsForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest)
        {
            await AuthorizeOrgAccessAsync(user, orgId, typeof(Incident));
            return await _incidentsRepo.GetIncidentsForOrgAsync(orgId, listRequest);
        }

        public async Task<InvokeResult> UpdateIncidentAsync(Incident incident, EntityHeader org, EntityHeader user)
        {
            ValidationCheck(incident, Actions.Create);
            await AuthorizeAsync(incident, AuthorizeResult.AuthorizeActions.Update, user, org);

            await _incidentsRepo.UpdateIncidentAsync(incident);

            return InvokeResult.Success;
        }
    }
}
