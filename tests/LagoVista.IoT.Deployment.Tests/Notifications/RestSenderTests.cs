﻿using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Notifications
{
    [TestClass]
    public class RestSenderTests : TestBase
    {
        IRestSender _restSender;

        [TestInitialize]
        public void Init()
        {
            _restSender = new RESTSender(SecureStorege.Object, AdminLogger, TagReplacer.Object);
        }



        [TestMethod]
        public async Task SendRestMessage()
        {
            var restNotif = new Rest()
            {

            };           

            await _restSender.SendAsync(restNotif, GetDevice(), GetLocation(), OrgEH, UserEH);
        }
    }
}
