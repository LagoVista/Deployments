// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8b6000afd669d1af5742f78c8fdd9cbada05f80fc66eb67b586d9a4de502484c
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using LagoVista.MediaServices.Models;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Users;
using System.Collections.Generic;
using System.Data.Common;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Incident_Title, DeploymentAdminResources.Names.Incident_Description,
        DeploymentAdminResources.Names.Incident_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-ae-critical",
        ListUIUrl: "/iotstudio/incidents", EditUIUrl: "/iotstudio/incident/{id}", CreateUIUrl: "/iotstudio/incident/add",
        GetListUrl: "/api/incidents", SaveUrl: "/api/incident", GetUrl: "/api/incident/{id}", DeleteUrl: "/api/incident/{id}", FactoryUrl: "/api/incident/factory")]
    public class Incident : EntityBase, ISummaryFactory, IIconEntity, IDescriptionEntity, IValidateable
    {
        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-ae-critical";

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceLibraryResources))]
        public string Description { get; set; }

        public EntityHeader IncidentProtocol { get; set; }

        public string OpenedTimeStamp { get; set; }

        public List<NotificationContact> DistributionList { get; set; } = new List<NotificationContact>();

        public EntityHeader ResolvedBy { get; set; }

        public string ResolvedTimeStamp { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Resources, FieldType: FieldTypes.MediaResources, ResourceType: typeof(DeploymentAdminResources))]
        public List<MediaResourceSummary> Resources { get; set; } = new List<MediaResourceSummary>();


        public List<IncidentStepResult> StepResults { get; set; } = new List<IncidentStepResult>();

        public SystemTestSummary CreateSummary()
        {
            return new SystemTestSummary()
            {
                Id = Id,
                Name = Name,
                Key = Key,
                Icon = Icon,
                IsPublic = IsPublic,
                Description = Description
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }       
    }

    public class IncidentStepResult
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public EntityHeader CompletedBy { get; set; }
        public EntityHeader IncidentProtocolStep { get; set; }
        public string TimeStamp { get; set; }
        public string Notes { get; set; }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Incidents_Title, DeploymentAdminResources.Names.Incident_Description,
        DeploymentAdminResources.Names.Incident_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-critical",
        ListUIUrl: "/iotstudio/incidents", EditUIUrl: "/iotstudio/incident/{id}", CreateUIUrl: "/iotstudio/incident/add",
        GetListUrl: "/api/incidents", SaveUrl: "/api/incident", GetUrl: "/api/incident/{id}", DeleteUrl: "/api/incident/{id}", FactoryUrl: "/api/incident/factory")]
    public class IncidentSummary : SummaryData
    {

    }
}
