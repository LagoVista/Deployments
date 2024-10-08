﻿using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public async Task PreparePageTest()
        {
            await _smsSender.PrepareMessage(GetNotification(), GetTestMode(), GetDevice(), GetLocation());
        }

        [TestMethod]
        public async Task SendTest()
        {
            var result = await _smsSender.SendAsync("dontcare", GetRecipient(), GetLinks(), false, OrgEH, UserEH);
            Assert.IsTrue(result.Successful, result.ErrorMessage);
        }
    }
}
