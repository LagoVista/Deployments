using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.MediaServices.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.SystemTest_Title, DeploymentAdminResources.Names.SystemTest_Title,
        DeploymentAdminResources.Names.SystemTestStep_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-ae-device-config",       
        FactoryUrl: "/api/systemtest/step/factory")]
    public class SystemTestStep : IValidateable, IFormDescriptor, IIDEntity, IIconEntity
    {
        public SystemTestStep()
        {
            Id = Guid.NewGuid().ToId();
        }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public string Name { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Key, HelpResource: LagoVistaCommonStrings.Names.Common_Key_Help, FieldType: FieldTypes.Key,
            RegExValidationMessageResource: LagoVistaCommonStrings.Names.Common_Key_Validation, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true)]
        public string Key { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-ae-device-config";


        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestStep_Summary, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string Summary { get; set; }

        
        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestStep_Details, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(DeploymentAdminResources), IsRequired: true, IsUserEditable: true)]
        public string Details { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestStep_WebLink, FieldType: FieldTypes.WebLink, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public string WebLink { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.SystemTestStep_Resources, FieldType: FieldTypes.MediaResources, ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true)]
        public List<MediaResourceSummary> Resources { get; set; } = new List<MediaResourceSummary>();

        public string Id { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(Summary),
                nameof(WebLink),
                nameof(Details),
                nameof(Resources)
            };
        }
    }
}
