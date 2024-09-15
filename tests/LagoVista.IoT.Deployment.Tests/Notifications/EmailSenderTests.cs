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
    public class EmailSenderTests : TestBase
    {
        IEmailSender _emailSender;

        [TestInitialize]
        public void Init()
        {
            _emailSender = new EmailSender(AdminLogger, TagReplacer.Object, EmailMessageSender.Object, LinkShortener.Object);
        }


    }
}
