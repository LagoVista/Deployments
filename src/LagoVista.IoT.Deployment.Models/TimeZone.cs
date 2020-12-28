using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class TimeZone : EntityHeader
    {
        public static IEnumerable<TimeZone> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(tz => new TimeZone() { Id = tz.Id, Text = tz.DisplayName });
        }
    }
}