// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fed468898d72cc1470238d5c40e174846ac5d66bde7cd07137788b4e4e82ab03
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Notifications
{
    [TestClass]
    public class LandingPageBuilderTests : TestBase
    {
        INotificationLandingPage _builder;

        [TestInitialize]
        public void Init()
        {
            _builder = new NotificationLandingPage(AdminLogger, AppConfig.Object, TagReplacer.Object, StaticPageService.Object);
        }

        [TestMethod]
        public async Task CreateLandingPage()
        {
            var result = await _builder.PreparePage(GetRaisedNotification().Id, null, GetNotification(), GetTestMode(), GetDevice(), GetLocation(), OrgEH, UserEH);
            Assert.IsTrue(result.Successful, result.ErrorMessage);
        }
    }
}
