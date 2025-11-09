// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 1bb576ee6edbe58b1661b233830fa7c2d5d7fb56bb2389af99fdb5f3b9fc512d
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Models.Resources;
using System;
using System.Collections.Generic;

namespace LagoVista.IoT.Deployment.Models
{
    [EntityDescription(DeploymentAdminDomain.DeploymentAdmin, DeploymentAdminResources.Names.MessageWatchDog_Exclusion_Title, DeploymentAdminResources.Names.MessageWatchDog_Exclusion_Help,
      DeploymentAdminResources.Names.MessageWatchDog_Exclusion_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel,  
        typeof(DeploymentAdminResources), Icon: "icon-pz-smartwatch-1", FactoryUrl: "/api/deviceconfig/watchdogexclusion/factory")]
    public class WatchdogExclusion: IFormDescriptor, IFormConditionalFields
    {
        public WatchdogExclusion()
        {
            Id = Guid.NewGuid().ToId();
            Start = 0;
            End = 759;
        }

        public string Id { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Name, FieldType: FieldTypes.Text, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Name { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Key, FieldType: FieldTypes.Key, ResourceType: typeof(DeploymentAdminResources), IsRequired: true)]
        public string Key { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.WatchdogExclusion_AllDay, HelpResource: DeploymentAdminResources.Names.WatchdogExclusion_AllDay_Help,
            FieldType: FieldTypes.CheckBox, ResourceType: typeof(DeploymentAdminResources))]
        public bool AllDay { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_Exclusion_Start, HelpResource: DeploymentAdminResources.Names.MessageWatchDog_Exclusion_Start_Help,
           ValidationRegEx: @"^[0-2]?[0-9]?[0-5]?[0-9]$", FieldType: FieldTypes.Decimal, ResourceType: typeof(DeploymentAdminResources))]
        public int Start { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.MessageWatchDog_Exclusion_End, HelpResource: DeploymentAdminResources.Names.MessageWatchDog_Exclusion_End_Help,
           ValidationRegEx: @"^[0-2]?[0-9]?[0-5]?[0-9]$", FieldType: FieldTypes.Decimal, ResourceType: typeof(DeploymentAdminResources))]
        public int End { get; set; }

        [FormField(LabelResource: DeploymentAdminResources.Names.Common_Description, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(DeploymentAdminResources))]
        public string Description { get; set; }

        public FormConditionals GetConditionalFields()
        {
            return new FormConditionals()
            {
                ConditionalFields = new List<string>() { nameof(Start), nameof(End) },
                Conditionals = new List<FormConditional>()
                {
                    new FormConditional()
                    {
                        Field = nameof(AllDay),
                        Value = "false",
                        VisibleFields = new List<string>() {nameof(Start),nameof(End)}
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
                nameof(Start), 
                nameof(End),
                nameof(Description),
            };
		}

		[CustomValidator]
        public void Validate(ValidationResult result)
        {
            var startHours = Start / 100;
            var startMinutes = Start - ((Start / 100) * 100);

            if (startHours > 23)
            {
                result.AddUserError($"Start hours must not be greater then 23, Start={Start / 100}:{Start % 100:00}");
            }

            if (startMinutes > 59)
            {
                result.AddUserError($"Minutes portion of start time must not be greater then 59, Start={Start / 100}:{Start % 100:00}");
            }

            var endHours = End / 100;
            var endMinutes = End - ((End / 100) * 100);

            if (endHours > 23)
            {
                result.AddUserError($"End hours must not be greater then 23, Start={End / 100}:{End % 100:00}");
            }

            if (endMinutes > 59)
            {
                result.AddUserError($"Minutes portion of end time must not be greater then 59, End={End / 100}:{End % 100:00}");
            }

            if (Start == End)
            {
                result.AddUserError($"Start time for exclusion must not equal end time, Start={Start / 100}:{Start % 100:00} End={End / 100}:{End % 100:00}.");
            }

            if (End < Start)
            {
                result.AddUserError($"Start time for exclusion must not be greather then the end time, Start={Start / 100}:{Start % 100:00} End={End / 100}:{End % 100:00}.");
            }
        }
    }
}
