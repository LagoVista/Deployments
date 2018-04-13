using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    enum RemoteDeploymentType
    {
        Normal,
        FaultTolerant,
        SingleInstance,
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.RemoteDeployment_Title, DeploymentAdminResources.Names.RemoteDeployment_Help, DeploymentAdminResources.Names.RemoteDeployment_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources))]
    public class RemoteDeployment : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity, IFormDescriptor
    {
        public RemoteDeployment()
        {
            Instances = new List<DeploymentInstance>();
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.RemoteDeployment_PrimaryMCP, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public DeploymentHost PrimaryMCP { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RemoteDeployment_SecondaryMCP, FieldType: FieldTypes.ChildItem, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public DeploymentHost SecondaryMCP { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, HelpResource: DeploymentAdminResources.Names.Common_Key_Help, FieldType: FieldTypes.Key, RegExValidationMessageResource: DeploymentAdminResources.Names.Common_Key_Validation, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.RemoteDeployment_Instances, FieldType: FieldTypes.ChildList, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public List<DeploymentInstance> Instances { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(DeploymentHost.Name),
                nameof(DeploymentHost.Key),
                nameof(DeploymentHost.Status),
                nameof(DeploymentHost.HostType),
                nameof(DeploymentHost.Size),
                nameof(DeploymentHost.DnsHostName),
                nameof(DeploymentHost.Ipv4Address),
                nameof(DeploymentHost.Subscription),
                nameof(DeploymentHost.CloudProvider),
                nameof(DeploymentHost.ContainerRepository),
                nameof(DeploymentHost.ContainerTag),
            };
        }

        public RemoteDeploymentSummary CreateSummary()
        {
            return new RemoteDeploymentSummary()
            {
                Id = Id,
                Name = Name,
                Key = Key,
                Description = Description,
                IsPublic = IsPublic
            };
        }

    }


    public class RemoteDeploymentSummary : SummaryData
    {

    }
}
