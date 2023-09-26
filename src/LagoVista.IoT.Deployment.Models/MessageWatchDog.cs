using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using LagoVista.IoT.DeviceMessaging.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.MessageWatchDog_Title, DeploymentAdminResources.Names.MessageWatchDog_Help,
     DeploymentAdminResources.Names.MessageWatchDog_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(DeploymentAdminResources),
      FactoryUrl: "/api/deviceconfig/messagewatchdog/factory")]
    public class MessageWatchDog : IFormDescriptor
    {
        public MessageWatchDog()
        {
            WeekdayExclusions = new List<WatchdogExclusion>();
            SaturdayExclusions = new List<WatchdogExclusion>();
            SundayExclusions = new List<WatchdogExclusion>();
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, FieldType: FieldTypes.Key, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchdog_Timeout, HelpResource: DeploymentAdminResources.Names.MessageWatchdog_Timeout_Help, 
            FieldType: FieldTypes.Decimal, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public int Timeout { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchdog_Timeout_Interval, HelpResource: DeploymentAdminResources.Names.MessageWatchdog_Timeout_Interval_Help, 
            WaterMark: DeploymentAdminResources.Names.MessageWatchdog_Timeout_Interval_Select, FieldType: FieldTypes.Picker, EnumType: typeof(TimeSpanIntervals), ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<TimeSpanIntervals> TimeoutInterval { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_StartupBuffer, HelpResource: DeploymentAdminResources.Names.MessageWatchDog_StartupBuffer_Help,
            FieldType: FieldTypes.Decimal, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public int StartupBufferMinutes { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Description { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_Message, WaterMark: DeploymentAdminResources.Names.MessageWatchDog_DeviceMessage_Select, 
            HelpResource: DeploymentAdminResources.Names.MessageWatchDog_Message_Help, FieldType: FieldTypes.EntityHeaderPicker, IsRequired:true, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<DeviceMessageDefinition> DeviceMessageDefinition { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_ErrorCode, WaterMark: DeploymentAdminResources.Names.MessageWatchDog_ErrorCode_Select,
            HelpResource: DeploymentAdminResources.Names.MessageWatchDog_ErrorCode_Help, FieldType: FieldTypes.EntityHeaderPicker, IsRequired:true, ResourceType: typeof(DeploymentAdminResources))]
        public EntityHeader<DeviceErrorCode> DeviceErrorCode {get; set;}

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_ExcludeHolidays, FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool ExcludeHolidays { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_WeekdayExclusions, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<WatchdogExclusion> WeekdayExclusions { get; set; }
        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_SaturdayExclusions, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<WatchdogExclusion> SaturdayExclusions { get; set; }
        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_SundayExclusions, FieldType: FieldTypes.ChildListInline, ResourceType: typeof(DeploymentAdminResources))]
        public List<WatchdogExclusion> SundayExclusions { get; set; }

		public List<string> GetFormFields()
		{
            return new List<string>()
            {
                nameof(Name),
				nameof(Key),
				nameof(Description),
				nameof(Timeout),
				nameof(TimeoutInterval),
				nameof(StartupBufferMinutes),
				nameof(DeviceMessageDefinition),
				nameof(DeviceErrorCode),
				nameof(ExcludeHolidays),
				nameof(WeekdayExclusions),
				nameof(SaturdayExclusions),
				nameof(SundayExclusions),
			};
		}
	}
}