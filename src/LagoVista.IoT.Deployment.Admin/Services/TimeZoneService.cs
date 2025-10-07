using LagoVista.Core.Interfaces;
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
    public class TimeZoneService : ITimeZoneServices
    {
        private static List<TimeZoneInfo> _timeZones;
        private static object _loadeLocker = new object();

        public List<TimeZoneInfo> GetTimeZones()
        {
            lock (_loadeLocker)
            {
                if (_timeZones == null)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = "LagoVista.IoT.Deployment.Admin.Data.TimeZones.json";

                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    using (var reader = new StreamReader(stream))
                    {
                        string jsonFile = reader.ReadToEnd();
                        _timeZones = JsonConvert.DeserializeObject<List<TimeZoneInfo>>(jsonFile);
                    }
                }

                return _timeZones;
            }
        }

        public TimeZoneInfo GetTimeZoneById(string id)
        {
            return GetTimeZones().FirstOrDefault(tz => tz.Id == id);
        }

        public List<EnumDescription> GetTimeZoneEnumOptions()
        {
            var options = GetTimeZones().Select(tz => new EnumDescription() { Id = tz.Id, Key = tz.Id, Label = tz.DisplayName, Name = tz.DisplayName }).ToList();
            options.Insert(0, new EnumDescription()
            {
                Id = "-1",
                Key = "-1",
                Label = DeploymentAdminResources.Common_SelectTimeZone,
                Name = DeploymentAdminResources.Common_SelectTimeZone,
            }); return options;
        }

    }
}
