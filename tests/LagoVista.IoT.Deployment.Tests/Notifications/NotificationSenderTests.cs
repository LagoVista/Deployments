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
    public class NotificationSenderTests :  TestBase
    {

        INotificationSender _sender;

        [TestInitialize]
        public void Init()
        {
            _sender = new NotificationSender(AdminLogger, DistroLibRepo.Object, NotificationTacker.Object, NotificationRepo.Object, OrgLocationRepo.Object, DeviceManager.Object, EmailSender.Object, SMSSender.Object, LandingPageBuilder.Object,
                AppUserRepo.Object, RepoManager.Object, COTSender.Object, RestSender.Object, MqttSender.Object, DeploymentInstanceRepo.Object);
        }

    }
}
