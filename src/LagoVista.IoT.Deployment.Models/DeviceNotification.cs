﻿using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.UserAdmin.Models.Orgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceNotification_Title, DeploymentAdminResources.Names.DeviceNotifications_Description,
            DeploymentAdminResources.Names.DeviceNotifications_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1",
            ListUIUrl: "/iotstudio/device/notifications", EditUIUrl: "/iotstudio/device/notification/{id}", CreateUIUrl: "/iotstudio/device/notification/add",
            GetListUrl: "/api/notifications", SaveUrl: "/api/notification", GetUrl: "/api/notification/{id}", DeleteUrl: "/api/notification/{id}", FactoryUrl: "/api/notification/factory")]
    public class DeviceNotification : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IFormDescriptor, IIconEntity, IFormDescriptorCol2, ICategorized, IFormConditionalFields, ISummaryFactory
    {

        public DeviceNotification()
        {
            Icon = "icon-fo-notification-1";
        }


        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_EmailSubject, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_EmailSubject_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public string EmailSubject { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_SendSMS, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendSMS { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_SendEmail,  FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendEmail { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_SMSContent, HelpResource:DeploymentAdminResources.Names.DeviceNotification_TagHelp, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources),
            ReplaceableTags: "Device Name-DeviceName;Device Id-DeviceId;Device Information Page-DeviceInfoPage;Device Location-DeviceLocation;Location Adminstrative Contact-Location_Admin_Contact;Device Technical Contact-Location_Technical_Contact;Device Summary-DeviceSummary;Notification Time Stamp-NotificationTimeStamp")]
        public string SmsContent { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_IncludeLandingPageContent, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IncludeLandingPage { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_LandingPageContent, HelpResource: DeploymentAdminResources.Names.DeviceNotification_TagHelp, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(DeploymentAdminResources),
            ReplaceableTags: "Device Name-DeviceName;Device Id-DeviceId;Device Information Page-DeviceInfoPage;Device Location-DeviceLocation;Location Adminstrative Contact-Location_Admin_Contact;Device Technical Contact-Location_Technical_Contact;Device Summary-DeviceSummary;Notification Time Stamp-NotificationTimeStamp")]
        public string LandingPageContent { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_EmailContent, HelpResource: DeploymentAdminResources.Names.DeviceNotification_TagHelp, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(DeploymentAdminResources),
            ReplaceableTags: "Device Name-DeviceName;Device Id-DeviceId;Device Information Page-DeviceInfoPage;Device Location-DeviceLocation;Location Adminstrative Contact-Location_Admin_Contact;Device Technical Contact-Location_Technical_Contact;Device Summary-DeviceSummary;Notification Time Stamp-NotificationTimeStamp")]
        public string EmailContent { get; set; }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(SmsContent), nameof(EmailSubject), nameof(EmailContent), nameof(LandingPageContent) },
                Conditionals = new List<FormConditional>()
                 {
                     new FormConditional()
                     {
                         Field = nameof(SendSMS),
                         Value = "true",
                         RequiredFields = new List<string>() { nameof(SmsContent)},
                         VisibleFields = new List<string>() { nameof(SmsContent)}
                     },
                     new FormConditional()
                     {
                         Field = nameof(SendEmail),
                         Value = "true",
                         RequiredFields = new List<string>() { nameof(EmailContent), nameof(EmailSubject) },
                         VisibleFields = new List<string>() { nameof(EmailContent), nameof(EmailSubject) }
                     },
                     new FormConditional()
                     {
                         Field = nameof(IncludeLandingPage),
                         Value = "true",
                         RequiredFields = new List<string>() { nameof(LandingPageContent) },
                         VisibleFields = new List<string>() { nameof(LandingPageContent) }
                     }
                 }
            };
        }

        public DeviceNotificationSummary CreateSummary()
        {
            return new DeviceNotificationSummary()
            {
                Id = Id,
                Icon = Icon,
                Name = Name,
                Description = Description,
                Key = Key,
                IsPublic = IsPublic
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(Category),
            };
        }

        public List<string> GetFormFieldsCol2()
        {
            return new List<string>()
            {
                nameof(SendSMS),
                nameof(SmsContent),
                nameof(SendEmail),
                nameof(EmailSubject),
                nameof(EmailContent),
                nameof(IncludeLandingPage),
                nameof(LandingPageContent)
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceNotifications_Title, DeploymentAdminResources.Names.DeviceNotifications_Description,
        DeploymentAdminResources.Names.DeviceNotifications_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1",
        ListUIUrl: "/iotstudio/device/notifications", EditUIUrl: "/iotstudio/device/notification/{id}", CreateUIUrl: "/iotstudio/device/notification/add",
        GetListUrl: "/api/notifications", SaveUrl: "/api/notification", GetUrl: "/api/notification/{id}", DeleteUrl: "/api/notification/{id}", FactoryUrl: "/api/notification/factory")]
    public class DeviceNotificationSummary : SummaryData
    {

    }

    public class RaisedDeviceNotification
    {
        public bool TestMode { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToId();
        public string NotificationKey { get; set; }
        public string DeviceId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceRepositoryId { get; set; }

        public List<EntityHeader> AdditionalUsers { get; set; }
        public List<ExternalContact> AdditionalExternalContacts { get; set; }

        public InvokeResult Validate()
        {
            var result = InvokeResult.Success;
            if (String.IsNullOrEmpty(NotificationKey))
                result.AddSystemError("Missing notification key on RaiseDeviceNotification");

            if(String.IsNullOrEmpty(DeviceId) && String.IsNullOrEmpty(DeviceUniqueId))
                result.AddSystemError("Missing DeviceId or DeviceUniqueId on RaiseDeviceNotification");

            if (String.IsNullOrEmpty(DeviceRepositoryId))
                result.AddSystemError("Missing device repository id on RaiseDeviceNotification");

            return result;
        }
    }
}
