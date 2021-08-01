using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{

    public class MiscController : Controller
    {
        private readonly ITimeZoneServices _timeZoneServices;

        public MiscController(ITimeZoneServices timeZoneServices)
        {
            _timeZoneServices = timeZoneServices ?? throw new NullReferenceException(nameof(timeZoneServices));
        }

        /// <summary>
        /// Misc services - get all known time zones
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/misc/timezones")]
        public IEnumerable<EntityHeader> GetSystemTimeZones()
        {
            var timeZones = _timeZoneServices.GetTimeZones();
            return timeZones.Select(tz => new EntityHeader() { Id = tz.Id, Text = tz.DisplayName });
        }
    }
}
