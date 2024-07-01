using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;

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

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.Incidents_Title, DeploymentAdminResources.Names.Incident_Description,
        DeploymentAdminResources.Names.Incident_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-critical",
        ListUIUrl: "/iotstudio/incidents", EditUIUrl: "/iotstudio/incident/{id}", CreateUIUrl: "/iotstudio/incident/add",
        GetListUrl: "/api/incidents", SaveUrl: "/api/incident", GetUrl: "/api/incident/{id}", DeleteUrl: "/api/incident/{id}", FactoryUrl: "/api/incident/factory")]
    public class IncidentSummary : SummaryData
    {

    }
}
