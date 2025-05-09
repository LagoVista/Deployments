﻿using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceAdmin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.ClientApp_Title, DeploymentAdminResources.Names.ClientApp_Help, DeploymentAdminResources.Names.ClientApp_Description, 
        EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-ae-apps",
        ListUIUrl: "/iotstudio/settings/clientapps", EditUIUrl: "/iotstudio/settings/clientapp/{id}", CreateUIUrl: "/iotstudio/settings/clientapp/add",
        SaveUrl: "/api/clientapp", GetUrl: "/api/clientapp/{id}", GetListUrl: "/api/clientapps", FactoryUrl: "/api/clientapp/factory", DeleteUrl: "/api/clientapp/{id}")]
    public class ClientApp : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase,  IValidateable, IFormDescriptor, ISummaryFactory, ICategorized
    {
        public ClientApp()
        {
            DeviceTypes = new ObservableCollection<EntityHeader>();
            DeviceConfigurations = new ObservableCollection<EntityHeader>();
            Icon = "icon-ae-apps";
        }



        public EntityHeader ClientAppUser { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, IsRequired: true, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; } = "icon-ae-apps";

        
        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.ClientApp_AppAuthKey1, FieldType: FieldTypes.Secret, SecureIdFieldName:nameof(AppAuthKeyPrimarySecureId), ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public String AppAuthKeyPrimary { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.ClientApp_AppAuthKey2, FieldType: FieldTypes.Secret, SecureIdFieldName: nameof(AppAuthKeySecondarySecureId), ResourceType: typeof(DeploymentAdminResources), IsUserEditable: false)]
        public String AppAuthKeySecondary { get; set; }

        public string AppAuthKeyPrimarySecureId { get; set; }
        public string AppAuthKeySecondarySecureId { get; set; }

        
        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(ClientApp.Name),
                nameof(ClientApp.Key),
                nameof(ClientApp.Icon),
                nameof(ClientApp.Category),
                nameof(ClientApp.Description),
                nameof(ClientApp.AppAuthKeyPrimary),
                nameof(ClientApp.AppAuthKeySecondary),
                nameof(ClientApp.DeploymentInstance),
                nameof(ClientApp.DeviceTypes),
                nameof(ClientApp.DeviceConfigurations),
            };
        }

        public ClientAppSummary CreateSummary()
        {
            return new ClientAppSummary()
            {
                Description = Description,
                Id = Id,
                IsPublic = IsPublic,
                Key = Key,
                Icon = Icon,
                Name = Name,
                InstanceId = DeploymentInstance?.Id,
                InstanceName = DeploymentInstance?.Text,
                Category = Category?.Text,
                CategoryId = Category?.Id,
                CategoryKey = Category?.Key,
            };
        }

        [FKeyProperty(nameof(DeploymentInstance),typeof(DeploymentInstance), "DeploymentInstance.Id = {0}", "")]
        [FormField(LabelResource: DeploymentAdminResources.Names.ClientApp_Instance,  FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeploymentAdminResources.Names.ClientApp_SelectInstance, 
            EntityHeaderPickerUrl: "/api/deployment/instances", 
            ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public EntityHeader DeploymentInstance { get; set; }

		[FormField(LabelResource: DeploymentAdminResources.Names.ClientApp_Kiosk, FieldType: FieldTypes.EntityHeaderPicker, WaterMark: DeploymentAdminResources.Names.ClientApp_Kiosk_Select, EntityHeaderPickerUrl: "/api/ui/kiosks", 
            ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
		public EntityHeader Kiosk { get; set; }


        [FKeyProperty(nameof(DeviceType), typeof(DeviceType), "DeviceTypes[*].Id = {0}", "")]
        [FormField(LabelResource: DeploymentAdminResources.Names.ClientApp_DeviceTypes, ChildListDisplayMember: "text", FieldType: FieldTypes.ChildListInlinePicker, EntityHeaderPickerUrl: "/api/devicetypes",
            ResourceType: typeof(DeploymentAdminResources))]
        public ObservableCollection<EntityHeader> DeviceTypes { get; set; }

        [FKeyProperty(nameof(DeviceConfiguration), typeof(DeviceConfiguration), "DeviceConfigurations[*].Id = {0}","")]
        [FormField(LabelResource: DeploymentAdminResources.Names.ClientApp_DeviceConfigs, ChildListDisplayMember: "text", FieldType: FieldTypes.ChildListInlinePicker, EntityHeaderPickerUrl: "/api/deviceconfigs",
            ResourceType: typeof(DeploymentAdminResources))]
        public ObservableCollection<EntityHeader> DeviceConfigurations { get; set; }

        [CustomValidator]
        public void Validate(ValidationResult result, Actions action)
        {
            if (action == Actions.Create)
            {
                if (String.IsNullOrEmpty(AppAuthKeyPrimary))
                {
                    result.AddUserError("AppAuthKeyPrimary is required when creating a client app.");
                }

                if (String.IsNullOrEmpty(AppAuthKeySecondary))
                {
                    result.AddUserError("AppAuthKeySecondary is required when creating a client app.");
                }
            }
            else if (action == Actions.Update)
            {
                if (String.IsNullOrEmpty(AppAuthKeyPrimary) && String.IsNullOrEmpty(AppAuthKeyPrimarySecureId))
                {
                    result.AddUserError("AppAuthKeyPrimary or AppAuthKeyPrimarySecureId is required when creating a client app.");
                }

                if (String.IsNullOrEmpty(AppAuthKeySecondary) && String.IsNullOrEmpty(AppAuthKeySecondarySecureId))
                {
                    result.AddUserError("AppAuthKeySecondary or AppAuthKeySecondarySecureId is required when creating a client app.");
                }
            }
        }

        Core.Interfaces.ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    public class ClientAppSecrets
    {
        public string AppAuthKeyPrimary { get; set; }
        public string AppAuthKeySecondary { get; set; }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.ClientApps_Title, DeploymentAdminResources.Names.ClientApp_Help, DeploymentAdminResources.Names.ClientApp_Description,
      EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-apps",
      SaveUrl: "/api/kioskclientapp/{id}", GetUrl: "/api/clientapp/{id}", GetListUrl: "/api/clientapps", FactoryUrl: "/api/clientapp/factory", DeleteUrl: "/api/clientapp/{id}")]
    public class ClientAppSummary : SummaryData
    {
        public string InstanceId { get; set; }
        public string InstanceName { get; set; }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.ClientApplications_Title, DeploymentAdminResources.Names.ClientApp_Help, DeploymentAdminResources.Names.ClientApp_Description,
      EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-apps",
      SaveUrl: "/api/kioskclientapp/{id}", GetUrl: "/api/clientapp/{id}", GetListUrl: "/api/clientapps", FactoryUrl: "/api/clientapp/factory", DeleteUrl: "/api/clientapp/{id}")]
    public class KioskClientAppSummary
	{
		public string KioskUrl { get; set; }
		public bool Successful { get; set; }
	}
}
