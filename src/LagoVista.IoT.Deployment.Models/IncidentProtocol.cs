using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.IncidentProtocol_Title, DeploymentAdminResources.Names.Incident_Description,
        DeploymentAdminResources.Names.Incident_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-ae-critical",
        ListUIUrl: "/iotstudio/settings/incidentprotocols", EditUIUrl: "/iotstudio/settings/incidentprotocol/{id}", CreateUIUrl: "/iotstudio/settingsincident/add",
        GetListUrl: "/api/incident/protocols", SaveUrl: "/api/incident/protocol", GetUrl: "/api/incident/protocol/{id}", DeleteUrl: "/api/incident/protocol/{id}", FactoryUrl: "/api/incident/protocol/factory")]
    public class IncidentProtocol : EntityBase, ISummaryFactory, IIconEntity, IDescriptionEntity, IValidateable, IFormDescriptor
    {
        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-ae-critical";

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeviceLibraryResources))]
        public string Description { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }

        public IncidentProtocolSummary CreateSummary()
        {
            return new IncidentProtocolSummary()
            {
                Id = Id,
                Name = Name,
                Key = Key,
                Icon = Icon,
                IsPublic = IsPublic,
                Description = Description
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Category),
                nameof(Icon),                
                nameof(Description),
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.IncidentProtocols_Title, DeploymentAdminResources.Names.Incident_Description,
        DeploymentAdminResources.Names.Incident_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-critical",
        ListUIUrl: "/iotstudio/protocol/incidents", EditUIUrl: "/iotstudio/incident/protocol/{id}", CreateUIUrl: "/iotstudio/incident/add",
        GetListUrl: "/api/incident/protocols", SaveUrl: "/api/incident/protocol", GetUrl: "/api/incident/protocol/{id}", DeleteUrl: "/api/incident/protocol/{id}", FactoryUrl: "/api/incident/protocol/factory")]
    public class IncidentProtocolSummary : SummaryData
    {
    }
}
