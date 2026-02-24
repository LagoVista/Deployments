// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ba5eced96a9ff5c0a196a413d107e1822eb642262ec1295c66aaab6ca36771e8
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models.Resources;
using LagoVista.MediaServices.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.IncidentProtocol_Title, DeploymentAdminResources.Names.IncidentProtocol_Description,
        DeploymentAdminResources.Names.IncidentProtocol_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-ae-critical", Cloneable: true,
        ListUIUrl: "/iotstudio/settings/incidentprotocols", EditUIUrl: "/iotstudio/settings/incidentprotocol/{id}", CreateUIUrl: "/iotstudio/settingsincident/add", 
        GetListUrl: "/api/incident/protocols", SaveUrl: "/api/incident/protocol", GetUrl: "/api/incident/protocol/{id}", DeleteUrl: "/api/incident/protocol/{id}", FactoryUrl: "/api/incident/protocol/factory")]
    public class IncidentProtocol : EntityBase, ISummaryFactory, IIconEntity, IDescriptionEntity, IValidateable, IFormDescriptor
    {
        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-ae-critical";


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Resources, FieldType: FieldTypes.MediaResources, ResourceType: typeof(DeploymentAdminResources))]
        public List<MediaResourceSummary> Resources { get; set; } = new List<MediaResourceSummary>();


        [FormField(LabelResource: DeploymentAdminResources.Names.IncidentProtocol_Steps, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/incident/protocol/step/factory", ResourceType: typeof(DeploymentAdminResources))]
        public List<IncidentProtocolStep> Steps { get; set; } = new List<IncidentProtocolStep>();

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
                nameof(Resources),
                nameof(Steps)
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

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.IncidentProtocolStep_Title, DeploymentAdminResources.Names.IncidentProtocolStep_Description,
    DeploymentAdminResources.Names.IncidentProtocolStep_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-device-config",
        FactoryUrl: "/api/incident/protocol/step/factory")]
    public class IncidentProtocolStep : IFormDescriptor, IIconEntity, IIDEntity
    {
        public string Id { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, IsRequired:true, ResourceType: typeof(DeploymentAdminResources))]
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, IsRequired:true, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-ae-device-config";

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, FieldType: FieldTypes.Key, IsRequired: true, ResourceType: typeof(DeploymentAdminResources))]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.IncidentProtocolStep_AssignedTo, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string AssignedTo { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Summary, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Summary { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Details, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(DeploymentAdminResources))]
        public string Details { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Resources, FieldType: FieldTypes.MediaResources, ResourceType: typeof(DeploymentAdminResources))]
        public List<MediaResourceSummary> Resources { get; set; } = new List<MediaResourceSummary>();

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(AssignedTo),
                nameof(Summary),
                nameof(Details),
                nameof(Resources),
            };            
        }
    }
}
