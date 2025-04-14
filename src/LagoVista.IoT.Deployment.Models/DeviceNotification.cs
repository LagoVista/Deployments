using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.UserAdmin.Models.Orgs;
using LagoVista.UserAdmin.Models.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceNotification_Title, DeploymentAdminResources.Names.DeviceNotifications_Description,
            DeploymentAdminResources.Names.DeviceNotifications_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1", Cloneable: true,
            ListUIUrl: "/iotstudio/device/notifications", EditUIUrl: "/iotstudio/device/notification/{id}", CreateUIUrl: "/iotstudio/device/notification/add",
            GetListUrl: "/api/notifications", SaveUrl: "/api/notification", GetUrl: "/api/notification/{id}", DeleteUrl: "/api/notification/{id}", FactoryUrl: "/api/notification/factory")]
    public class DeviceNotification : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IValidateable, IFormDescriptor, IIconEntity, IFormDescriptorCol2, ICategorized, IFormConditionalFields, ISummaryFactory, ICustomerOwnedEntity
    {

        public DeviceNotification()
        {
            Icon = "icon-fo-notification-1";
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotification_EmailSubject, HelpResource: DeploymentAdminResources.Names.DeviceNotification_EmailSubject_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public string EmailSubject { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_SendSMS, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendSMS { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_SendEmail,  FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendEmail { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_SMSContent, HelpResource:DeploymentAdminResources.Names.DeviceNotification_SMS_Help, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string SmsContent { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_ForwardToParentDevice, HelpResource:DeploymentAdminResources.Names.DeviceNotifications_ForwardToParentDevice_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool ForwardToParentDevice { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_ForwardToParentBody, HelpResource: DeploymentAdminResources.Names.DeviceNotification_SMS_Help, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string ForwardToParentDeviceBody { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_IncludeLandingPageContent, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool IncludeLandingPage { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_LandingPageContent, HelpResource: DeploymentAdminResources.Names.DeviceNotification_TagHelp, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(DeploymentAdminResources),
            ReplaceableTags: "Device Name-DeviceName;Device Id-DeviceId;Device Information Page-DeviceInfoPage;Device Location-DeviceLocation;Location Adminstrative Contact-Location_Admin_Contact;Device Technical Contact-Location_Technical_Contact;Device Summary-DeviceSummary;Street Address-DeviceStreetAddress;Location Name-DeviceLocationName;Notification Time Stamp-NotificationTimeStamp;Phone Number-PhoneNumber;Last Contact Time-LastContactTime;Time Since Last Contact-TimeSinceLastContact;Device Sensors-DeviceSensors")]
        public string LandingPageContent { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Customer, FieldType: FieldTypes.CustomerPicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader Customer { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_SharedTemplate, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SharedTemplate { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotification_ForwardDevice, HelpResource:DeploymentAdminResources.Names.DeviceNotification_ForwardDevice_Help, WaterMark: DeploymentAdminResources.Names.DeviceNotification_ForwardDevice_Select, 
            FieldType: FieldTypes.DevicePicker, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader ForwardDevice { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList_Help,
            WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList_Select, FieldType: FieldTypes.EntityHeaderPicker,
            EntityHeaderPickerUrl: "/api/distros",
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader DistroList { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_Escalation_Notification, HelpResource:DeploymentAdminResources.Names.DeviceNotifications_Escalation_Notification_Help, WaterMark:DeploymentAdminResources.Names.DeviceNotification_EscalationNotification_Select, FieldType: FieldTypes.EntityHeaderPicker, EntityHeaderPickerUrl: "/api/notifications", ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader EscalationNotification { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_EmailContent, HelpResource: DeploymentAdminResources.Names.DeviceNotification_TagHelp, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(DeploymentAdminResources),
            ReplaceableTags: "Device Name-DeviceName;Device Id-DeviceId;Device Information Page-DeviceInfoPage;Device Location-DeviceLocation;Location Adminstrative Contact-Location_Admin_Contact;Device Technical Contact-Location_Technical_Contact;Device Summary-DeviceSummary;Street Address-DeviceStreetAddress;Location Name-DeviceLocationName;Notification Time Stamp-NotificationTimeStamp;Phone Number-PhoneNumber;Last Contact Time-LastContactTime;Time Since Last Contact-TimeSinceLastContact;Device Sensors-DeviceSensors")]
        public string EmailContent { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_CotNotifications, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/notification/cot/factory", ResourceType: typeof(DeploymentAdminResources))]
        public List<CursorOnTarget> CotNotifications { get; set; } = new List<CursorOnTarget>();

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_RestNotifications, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/notification/rest/factory", ResourceType: typeof(DeploymentAdminResources))]
        public List<Rest> RestNotifications { get; set; } = new List<Rest>();

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_MqttNotifications, FieldType: FieldTypes.ChildListInline, FactoryUrl: "/api/notification/mqtt/factory", ResourceType: typeof(DeploymentAdminResources))]

        public List<Mqtt> MqttNotifications { get; set; } = new List<Mqtt>();

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceNotifications_Contacts,  ChildListDisplayMembers: "FirstName,LastName", FieldType: FieldTypes.ChildListInline, EntityHeaderPickerUrl: "/api/distro/externalcontact/factory", ResourceType: typeof(DeploymentAdminResources))]
        public List<ExternalContact> Contacts { get; set; } = new List<ExternalContact>();


        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(SmsContent), nameof(EmailSubject), nameof(EmailContent), nameof(LandingPageContent), nameof(ForwardToParentDeviceBody) },
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
                         Field = nameof(ForwardToParentDevice),
                         Value = "true",
                         RequiredFields = new List<string>() { nameof(ForwardToParentDeviceBody)},
                         VisibleFields = new List<string>() { nameof(ForwardToParentDeviceBody) }
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
                IsPublic = IsPublic,
                SharedTemplate = SharedTemplate,
                Custmer = Customer?.Text,
                CustomerId = Customer?.Id,
                Category = Category?.Text,
                CategoryId = Category?.Id,
                CategoryKey = Category?.Key,
            };
        }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(Name),
                nameof(Key),
                nameof(Icon),
                nameof(SharedTemplate),
                nameof(Customer),
                nameof(Category),
                nameof(RestNotifications),
                nameof(MqttNotifications),
                nameof(CotNotifications),
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
                nameof(DistroList),
                nameof(ForwardDevice),
                nameof(ForwardToParentDevice),
                nameof(ForwardToParentDeviceBody),
                nameof(IncludeLandingPage),
                nameof(LandingPageContent),
                nameof(EscalationNotification),
                nameof(Contacts)
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceNotifications_Title, DeploymentAdminResources.Names.DeviceNotifications_Description,
        DeploymentAdminResources.Names.DeviceNotifications_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-fo-notification-1",
        ListUIUrl: "/iotstudio/device/notifications", EditUIUrl: "/iotstudio/device/notification/{id}", CreateUIUrl: "/iotstudio/device/notification/add", Cloneable: true,
        GetListUrl: "/api/notifications", SaveUrl: "/api/notification", GetUrl: "/api/notification/{id}", DeleteUrl: "/api/notification/{id}", FactoryUrl: "/api/notification/factory")]
    public class DeviceNotificationSummary : SummaryData
    {
        public bool SharedTemplate { get; set; }
        public string CustomerId { get; set; }
        public string Custmer { get; set; }
    }

    public class RaisedDeviceNotification
    {
        /// <summary>
        /// Send the notifications, but include a banner that declares we are in test mode.
        /// </summary>
        public bool TestMode { get; set; }

        /// <summary>
        /// Process the notification, however to not send to any recipients
        /// </summary>
        public bool DryRun { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToId();
        public string NotificationKey { get; set; }
        public string DeviceId { get; set; }
        public string DeviceUniqueId { get; set; }
        public string DeviceRepositoryId { get; set; }

        public string DeviceErrorId { get; set; }

        public bool Escalate { get; set; }

        public List<EntityHeader> AdditionalUsers { get; set; } = new List<EntityHeader>();
        public List<ExternalContact> AdditionalExternalContacts { get; set; } = new List<ExternalContact>();

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
