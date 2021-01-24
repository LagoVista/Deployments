using LagoVista.IoT.Deployment.Admin.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.TimeZones
{
    [TestClass]
    public class TimeZoneTests
    {
        [TestMethod]
        public void GetTimeZones()
        {
            var services = new TimeZoneService();
            var tzs = services.GetTimeZones();
            Assert.AreEqual(140, tzs.Count);
        }

        [TestMethod]
        public void GetTimeZoneById()
        {
            var services = new TimeZoneService();
            var tzs = services.GetTimeZones();

            var timeZone = tzs[5];

           var tzLookup = services.GetTimeZoneById(timeZone.Id);
            Assert.IsNotNull(tzLookup);
            Assert.AreEqual(timeZone.DisplayName, tzLookup.DisplayName);
        }
    }
}
