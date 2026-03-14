using LagoVista.Core.Interfaces;
using LagoVista.Core.Models.DateTimeTypes;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.Deployment.Admin.Resources;
using LagoVista.IoT.Deployment.Models.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LagoVista.IoT.Deployment.Admin.Managers
{
    [CriticalCoverage]
    public class TimeZoneService : ITimeZoneServices
    {
        private static readonly object _loadLocker = new object();
        private static List<TimeZoneReference> _timeZoneReferences;
        private static List<TimeZoneInfo> _timeZones;

        public List<TimeZoneInfo> GetTimeZones()
        {
            EnsureLoaded();
            return _timeZones;
        }

        public TimeZoneInfo GetTimeZoneById(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                return null;

            EnsureLoaded();
            return _timeZones.FirstOrDefault(tz => tz.Id == id);
        }

        public TimeZoneInfo GetTimeZoneByIntId(int intId)
        {
            EnsureLoaded();

            var reference = _timeZoneReferences.FirstOrDefault(tz => tz.IntId == intId);
            if (reference == null)
                return null;

            return _timeZones.FirstOrDefault(tz => tz.Id == reference.Id);
        }

        public List<EnumDescription> GetTimeZoneEnumOptions()
        {
            EnsureLoaded();

            var options = _timeZoneReferences
                .Select(tz => new EnumDescription
                {
                    Id = tz.IntId.ToString(),
                    Key = tz.IntId.ToString(),
                    Label = tz.DisplayName,
                    Name = tz.DisplayName
                })
                .ToList();

            options.Insert(0, new EnumDescription
            {
                Id = "-1",
                Key = "-1",
                Label = DeploymentAdminResources.Common_SelectTimeZone,
                Name = DeploymentAdminResources.Common_SelectTimeZone,
            });

            return options;
        }

        private static void EnsureLoaded()
        {
            lock (_loadLocker)
            {
                if (_timeZoneReferences != null && _timeZones != null)
                    return;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "LagoVista.IoT.Deployment.Admin.Data.TimeZones.json";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    var jsonFile = reader.ReadToEnd();

                    _timeZoneReferences = JsonConvert.DeserializeObject<List<TimeZoneReference>>(jsonFile)
                        ?? new List<TimeZoneReference>();

                    _timeZones = _timeZoneReferences
                        .Select(BuildTimeZoneInfo)
                        .ToList();
                }
            }
        }

        private static TimeZoneInfo BuildTimeZoneInfo(TimeZoneReference reference)
        {
            var baseUtcOffset = TimeSpan.Parse(reference.BaseUtcOffset);

            var adjustmentRules = reference.AdjustmentRules == null || !reference.AdjustmentRules.Any()
                ? null
                : reference.AdjustmentRules.Select(BuildAdjustmentRule).ToArray();

            return TimeZoneInfo.CreateCustomTimeZone(
                reference.Id,
                baseUtcOffset,
                reference.DisplayName,
                reference.StandardName,
                reference.DaylightName,
                adjustmentRules);
        }

        private static TimeZoneInfo.AdjustmentRule BuildAdjustmentRule(TimeZoneAdjustmentRuleReference reference)
        {
            return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
               reference.DateStart.Date,
               reference.DateEnd.Date,
               reference.DaylightDelta,
               BuildTransitionTime(reference.DaylightTransitionStart),
               BuildTransitionTime(reference.DaylightTransitionEnd));
        }

        private static TimeZoneInfo.TransitionTime BuildTransitionTime(TimeZoneTransitionTimeReference reference)
        {
            var timeOfDay = reference.TimeOfDay;

            if (reference.IsFixedDateRule)
            {
                return TimeZoneInfo.TransitionTime.CreateFixedDateRule(
                    timeOfDay,
                    reference.Month,
                    reference.Day);
            }

            return TimeZoneInfo.TransitionTime.CreateFloatingDateRule(
                timeOfDay,
                reference.Month,
                reference.Week,
                reference.DayOfWeek);
        }

        public TimeZoneReference GetTimeZoneReferenceByIntId(int intId)
        {
            EnsureLoaded();

            var timeZone = _timeZoneReferences.FirstOrDefault(tz => tz.IntId == intId);
            if (timeZone == null)
                throw new InvalidOperationException($"Unknown timezone IntId [{intId}].");

            return timeZone;
        }

        public List<TimeZoneReference> GetTimeZoneReferences()
        {
            EnsureLoaded();
            return _timeZoneReferences;
        }
    }
}