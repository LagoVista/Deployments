// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ee82ca1d3d35d95089680d4eed6f41d44b7408e2f62027a9bc7ffe5cfe49d1c1
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

        [TestMethod]
        public async Task TestSend()
        {
            var mqtt = new Mqtt()
            {
                Address = Environment.GetEnvironmentVariable("DEV_MQTT_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("DEV_MQTT_PORT")),
                PasswordSecretId = Environment.GetEnvironmentVariable("ABC1234"),
                UserName = Environment.GetEnvironmentVariable("DEV_MQTT_USERNAME"),
                Password = Environment.GetEnvironmentVariable("DEV_MQTT_PASSWORD"), // typically this will be null, you can see the Mock setup will return the value entered here.
                Topic = "msg/one/two",
                Payload = "abc1234",
            };

            SecureStorege.Setup(ss => ss.GetSecretAsync(It.IsAny<EntityHeader>(), It.Is<string>(val => val == mqtt.PasswordSecretId), UserEH)).ReturnsAsync(InvokeResult<string>.Create(mqtt.Password));

            await _mqttSender.SendAsync(mqtt, GetDevice(), GetLocation(), OrgEH, UserEH);
        }
    }
}
