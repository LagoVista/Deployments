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
    public class SMSSenderTests : TestBase
    {
        ISMSSender _smsSender;

        [TestInitialize]
        public void Init() 
        {
            _smsSender = new SMSSender(AdminLogger, AppConfig.Object, TagReplacer.Object, LinkShortener.Object, SMSMessageSender.Object);
        }

    }
}
