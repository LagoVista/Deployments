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
    }
}
