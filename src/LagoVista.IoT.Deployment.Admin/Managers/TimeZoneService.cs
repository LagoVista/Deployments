using LagoVista.IoT.Deployment.Admin.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
    }
}
