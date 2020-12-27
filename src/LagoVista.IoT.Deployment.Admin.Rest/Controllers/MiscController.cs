using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{

    public class MiscController : Controller
    {
        /// <summary>
        /// Misc services - get all known time zones
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/misc/timezones")]
        public IEnumerable<Deployment.Models.TimeZone> GetSystemTimeZones()
        {
            return Deployment.Models.TimeZone.GetSystemTimeZones();
        }
    }
}
