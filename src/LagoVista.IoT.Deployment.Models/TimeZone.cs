using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class TimeZone
    {
        public string Id { get; set; }
        public string Name { get; set; }
    
        public static IEnumerable<TimeZone> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(tz => new TimeZone() { Id = tz.Id, Name = tz.DisplayName });
        }
    }
}
