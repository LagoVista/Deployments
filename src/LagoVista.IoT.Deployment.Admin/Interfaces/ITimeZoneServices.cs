using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface ITimeZoneServices
    {
        List<TimeZoneInfo> GetTimeZones();
        TimeZoneInfo GetTimeZoneById(string id);
    }
}
