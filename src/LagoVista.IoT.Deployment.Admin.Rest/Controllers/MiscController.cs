// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d25bb9dd7ab0ce1337d2c67e7ef88dd89ee35486f2eff1144970d4d55e10cc71
// IndexVersion: 0
// --- END CODE INDEX META ---
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
