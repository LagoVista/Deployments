using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.Logging;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models
{
    public enum TimeSpanIntervals
    {
        [EnumLabel(DeviceErrorCode.DeviceErrorCode_NotApplicable, DeploymentAdminResources.Names.DeviceErrorCode_NotApplicable, typeof(DeploymentAdminResources))]
        NotApplicable,

        [EnumLabel(DeviceErrorCode.DeviceErrorCode_Minutes, DeploymentAdminResources.Names.DeviceErrorCode_Minutes, typeof(DeploymentAdminResources))]
        Minutes,

        [EnumLabel(DeviceErrorCode.DeviceErrorCode_Hours, DeploymentAdminResources.Names.DeviceErrorCode_Hours, typeof(DeploymentAdminResources))]
        Hours,

        [EnumLabel(DeviceErrorCode.DeviceErrorCode_Days, DeploymentAdminResources.Names.DeviceErrorCode_Days, typeof(DeploymentAdminResources))]
        Days,
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceErrorCode_Title, DeploymentAdminResources.Names.DeviceErrorCode_Help,
        DeploymentAdminResources.Names.DeviceErrorCode_Description, EntityDescriptionAttribute.EntityTypes.CoreIoTModel, typeof(DeploymentAdminResources), Icon: "icon-ae-error-1",
        ListUIUrl: "/iotstudio/device/errorcodes", EditUIUrl: "/iotstudio/device/errorcode/{id}", CreateUIUrl: "/iotstudio/device/errorcode/add",
        GetListUrl: "/api/errorcodes", SaveUrl: "/api/errorcode", GetUrl: "/api/errorcode/{id}", DeleteUrl: "/api/errorcode/{id}", FactoryUrl: "/api/errorcode/factory")]
    public class DeviceErrorCode : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase,  IValidateable, IFormDescriptor, IIconEntity, IFormDescriptorCol2, IFormConditionalFields, ISummaryFactory, ICategorized
    {
        public const string DeviceErrorCode_NotApplicable = "na";
        public const string DeviceErrorCode_Minutes = "minutes";
        public const string DeviceErrorCode_Hours = "hours";
        public const string DeviceErrorCode_Days = "days";

        public DeviceErrorCode()
        {
            Id = Guid.NewGuid().ToId();
            NotificationIntervalTimeSpan = EntityHeader<TimeSpanIntervals>.Create(TimeSpanIntervals.NotApplicable);
            AutoexpireTimespan = EntityHeader<TimeSpanIntervals>.Create(TimeSpanIntervals.NotApplicable);
            Icon = "icon-ae-error-1";
            NotifyOnRaise = true;
            NotifyOnClear = true;
        }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Category, FieldType: FieldTypes.Category, WaterMark: DeploymentAdminResources.Names.Common_Category_Select, ResourceType: typeof(DeploymentAdminResources), IsRequired: false, IsUserEditable: true)]
        public EntityHeader Category { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_TriggerOnEachOccurrence, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_TriggerOnEachOccurrence_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool TriggerOnEachOccurrence { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_EmailSubject, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_EmailSubject_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public string EmailSubject { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(DeploymentAdminResources))]
        public string Icon { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_NotifyOnRaise, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool NotifyOnRaise { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_NotifyOnClear, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool NotifyOnClear { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_SendSMS, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendSMS { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_SendEmail, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendEmail { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList_Help,
            WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList_Select, FieldType: FieldTypes.EntityHeaderPicker,
            EntityHeaderPickerUrl: "/api/distros",
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader DistroList { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_DeviceNotification, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_DeviceNotification_Help,
            WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_DeviceNotification_Select, FieldType: FieldTypes.EntityHeaderPicker,
            EntityHeaderPickerUrl: "/api/notifications",
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader DeviceNotification { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate,
            HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate_Help,
            EntityHeaderPickerUrl: "/api/fslite/tickets/templates",
            WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate_Select, FieldType: FieldTypes.EntityHeaderPicker,
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader ServiceTicketTemplate { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_IncidentProtocol,
            HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_IncidentProtocol_Help,
            EntityHeaderPickerUrl: "/api/incident/protocols",
            WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_IncidentProtocol_Select, FieldType: FieldTypes.EntityHeaderPicker,
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader IncidentProtocol { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_NotificationIntervalQuantity, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public double? NotificationIntervalQuantity { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_NotificationInterval, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_NotificationInterval_Help, WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_SelectTimespan, FieldType: FieldTypes.Picker, EnumType: typeof(TimeSpanIntervals), ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<TimeSpanIntervals> NotificationIntervalTimeSpan { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_AutoExpiresTimespanQuantity, FieldType: FieldTypes.Decimal, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public double? AutoexpireTimespanQuantity { get; set; }


        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_AutoExpiresTimespan, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_AutoExpiresTimespan_Help, WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_SelectTimespan, FieldType: FieldTypes.Picker, EnumType: typeof(TimeSpanIntervals), ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<TimeSpanIntervals> AutoexpireTimespan { get; set; }

        public DeviceErrorCodeSummary CreateSummary()
        {
            return new DeviceErrorCodeSummary()
            {
                Description = Description,
                Id = Id,
                Icon = Icon,
                Name = Name,
                Key = Key,
                IsPublic = IsPublic,
                Category = Category
            };
        }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(TriggerOnEachOccurrence), nameof(EmailSubject), nameof(NotificationIntervalQuantity), nameof(AutoexpireTimespanQuantity) },
                Conditionals = new List<FormConditional>()
               {
                    new FormConditional()
                    {
                         Field = nameof(ServiceTicketTemplate),
                         Value = "*",
                         VisibleFields = new List<string>() { nameof(TriggerOnEachOccurrence) },
                    },
                    new FormConditional()
                    {
                         Field = nameof(AutoexpireTimespan),
                         Value = DeviceErrorCode_NotApplicable,
                         NotEquals = true,
                         VisibleFields = new List<string>() {  nameof(AutoexpireTimespanQuantity)},
                    },
                      new FormConditional()
                    {
                         Field = nameof(NotificationIntervalTimeSpan),
                         Value = DeviceErrorCode_NotApplicable,
                         NotEquals = true,
                         VisibleFields = new List<string>() {  nameof(NotificationIntervalQuantity) },
                    },
                    new FormConditional()
                    {
                         Field = nameof(SendEmail),
                         Value = "true",
                         VisibleFields = new List<string>() { nameof(EmailSubject)},
                         RequiredFields = new List<string>() {nameof(EmailSubject)},
                    }
               }
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
                nameof(ServiceTicketTemplate),
                nameof(TriggerOnEachOccurrence),
                nameof(IncidentProtocol),
                nameof(DistroList),
                nameof(DeviceNotification),
                nameof(SendEmail),
                nameof(EmailSubject),
                nameof(SendSMS),
                nameof(NotifyOnRaise),
                nameof(NotifyOnClear),
            };
        }

        public List<string> GetFormFieldsCol2()
        {
            return new List<string>()
            {
                nameof(AutoexpireTimespan),
                nameof(AutoexpireTimespanQuantity),
                nameof(NotificationIntervalTimeSpan),
                nameof(NotificationIntervalQuantity),
                nameof(Description),
            };
        }

        [CustomValidator]
        public void Validate(ValidationResult result)
        {
            if (NotificationIntervalTimeSpan != EntityHeader<TimeSpanIntervals>.Create(TimeSpanIntervals.NotApplicable) &&
                (!NotificationIntervalQuantity.HasValue || NotificationIntervalQuantity.Value <= 0))
            {
                result.AddUserError("You must specify a notification interval quantity.");
            }

            if (AutoexpireTimespan.Value != TimeSpanIntervals.NotApplicable &&
                (!AutoexpireTimespanQuantity.HasValue || AutoexpireTimespanQuantity.Value < 0))
            {
                result.AddUserError("You must specify a positive, non-zero auto expire value.");
            }
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.DeviceErrorCodes_Title, DeploymentAdminResources.Names.DeviceErrorCode_Help,
      DeploymentAdminResources.Names.DeviceErrorCode_Description, EntityDescriptionAttribute.EntityTypes.Summary, typeof(DeploymentAdminResources), Icon: "icon-ae-error-1",
      GetListUrl: "/api/errorcodes", SaveUrl: "/api/errorcode", GetUrl: "/api/errorcode/{id}", DeleteUrl: "/api/errorcode/{id}", FactoryUrl: "/api/errorcode/factory")]
    public class DeviceErrorCodeSummary : CategorizedSummaryData
    {

    }
}

namespace LagoVista
{
    public static class TImeSpanExtensions
    {
        public static TimeSpan ToTimeSpan(this TimeSpanIntervals interval, double quantity)
        {
            switch (interval)
            {
                case TimeSpanIntervals.Minutes: return TimeSpan.FromMinutes(quantity);
                case TimeSpanIntervals.Hours: return TimeSpan.FromHours(quantity);
                case TimeSpanIntervals.Days: return TimeSpan.FromDays(quantity);
            }

            throw new NotSupportedException($"Unknown case {interval} - note creating a time span from NotApplicable is not supported.");
        }

        public static string AddTimeSpan(this TimeSpanIntervals interval, double quantity, DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.UtcNow;
            }

            return date.Value.Add(interval.ToTimeSpan(quantity)).ToJSONString();
        }
    }
}
