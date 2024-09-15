using LagoVista.IoT.Deployment.Admin;
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
    public class MqttSenderTests : TestBase
    {
        IMqttSender _mqttSender;

        [TestInitialize]
        public void Init()
        {
            _mqttSender = new MQTTSender(SecureStorege.Object, AdminLogger, TagReplacer.Object);
        }
    }
}
