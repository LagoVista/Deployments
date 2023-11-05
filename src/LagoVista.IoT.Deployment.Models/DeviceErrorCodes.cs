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
        DeploymentAdminResources.Names.DeviceErrorCode_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources), 
        GetListUrl: "/api/errorcodes", SaveUrl: "/api/errorcode", GetUrl: "/api/errorcode/{id}", DeleteUrl: "/api/errorcodes", FactoryUrl: "/api/errorcode/factory")]
    public class DeviceErrorCode : LagoVista.IoT.DeviceAdmin.Models.IoTModelBase, IOwnedEntity, IValidateable, IKeyedEntity, INoSQLEntity, IFormDescriptor, IFormDescriptorCol2, IFormConditionalFields
    {
        public const string DeviceErrorCode_NotApplicable = "na";
        public const string DeviceErrorCode_Minutes = "minutes";
        public const string DeviceErrorCode_Hours = "hours";
        public const string DeviceErrorCode_Days = "days";

        public DeviceErrorCode()
        {
            Id = Guid.NewGuid().ToString();
            NotificationIntervalTimeSpan = EntityHeader<TimeSpanIntervals>.Create(TimeSpanIntervals.NotApplicable);
            AutoexpireTimespan = EntityHeader<TimeSpanIntervals>.Create(TimeSpanIntervals.NotApplicable);
        }
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }

        public bool IsPublic { get; set; }
        public EntityHeader OwnerOrganization { get; set; }
        public EntityHeader OwnerUser { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_ErrorCode, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_TriggerOnEachOccurrence, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_TriggerOnEachOccurrence_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool TriggerOnEachOccurrence { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_EmailSubject, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_EmailSubject_Help, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: false)]
        public string EmailSubject { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_SendSMS, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendSMS { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_SendEmail, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool SendEmail { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList, HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList_Help, 
            WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_DistributionList_Select, FieldType: FieldTypes.EntityHeaderPicker,
            EntityHeaderPickerUrl:"/api/distros",
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader DistroList { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate, 
            HelpResource: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate_Help,
            EntityHeaderPickerUrl: "/api/fslite/tickets/templates",
            WaterMark: DeploymentAdminResources.Names.DeviceErrorCode_TicketTemplate_Select, FieldType: FieldTypes.EntityHeaderPicker, 
            ResourceType: typeof(DeploymentAdminResources), IsUserEditable: true, IsRequired: false)]
        public EntityHeader ServiceTicketTemplate { get; set; }


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
                Name = Name,
                Key = Key,
                IsPublic = IsPublic
            };
        }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
               ConditionalFields = new List<string>() { nameof(SendEmail), nameof(SendSMS), nameof(EmailSubject), nameof(NotificationIntervalTimeSpan), nameof(NotificationIntervalQuantity) },
               Conditionals = new List<FormConditional>()
               {
                    new FormConditional()
                    {
                         Field = nameof(DistroList),
                         Value = "*",
                         VisibleFields = new List<string>() { nameof(SendEmail), nameof(SendSMS), nameof(NotificationIntervalQuantity), nameof(NotificationIntervalTimeSpan)},
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
				nameof(ServiceTicketTemplate),
		        nameof(DistroList),
                nameof(SendEmail),
                nameof(EmailSubject),
                nameof(SendSMS)
			};
		}

        public List<string> GetFormFieldsCol2()
        {
            return new List<string>()
            {
                nameof(TriggerOnEachOccurrence),
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
            if (EntityHeader.IsNullOrEmpty(DistroList))
            {
                NotificationIntervalTimeSpan = EntityHeader<TimeSpanIntervals>.Create(TimeSpanIntervals.NotApplicable);
                NotificationIntervalQuantity = null;
            }
            else
            {
                if (NotificationIntervalTimeSpan != EntityHeader<TimeSpanIntervals>.Create(TimeSpanIntervals.NotApplicable) &&
                   (!NotificationIntervalQuantity.HasValue || NotificationIntervalQuantity.Value <= 0))
                {
                    result.AddUserError("You must specify a notification interval quantity.");
                }
            }

            if (AutoexpireTimespan.Value != TimeSpanIntervals.NotApplicable &&
                (!AutoexpireTimespanQuantity.HasValue || AutoexpireTimespanQuantity.Value < 0))
            {
                result.AddUserError("You must specify a positive, non-zero auto expire value.");
            }
        }
    }

    public class DeviceErrorCodeSummary: SummaryData
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
            if(!date.HasValue)
            {
                date = DateTime.UtcNow;
            }

            return date.Value.Add(interval.ToTimeSpan(quantity)).ToJSONString();
        }
    }
}
