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
            _sender = new NotificationSender(AdminLogger, DistroLibRepo.Object, NotificationTacker.Object, NotificationRepo.Object, OrgLocationRepo.Object, DeviceManager.Object, EmailSender.Object, SMSSender.Object, LandingPageBuilder.Object, OrgRepo.Object,
                                RasiedDeviceNotificationRepo.Object, BackgroundTaskQueue.Object, DeviceConfigHelper.Object, AppUserRepo.Object, RepoManager.Object, COTSender.Object, RestSender.Object, MqttSender.Object, DeploymentInstanceRepo.Object, TimeZoneService.Object);
        }


        [TestMethod]
        public async Task SendNotificationAsync()
        {
            var result =  await _sender.RaiseNotificationAsync(GetRaisedNotification(), OrgEH, UserEH);
            Assert.AreEqual(result.Successful, result.ErrorMessage);
        }

        [TestMethod]
        public void UniqueNotificationsTest()
        {
            var contacts = new List<NotificationRecipient>()
            {
                new NotificationRecipient() {LastName = "1", Phone = "12345", Email = "abc@foo.com"},
                new NotificationRecipient() {LastName = "2", Phone = "12345", Email = "abc@foo.com"},
                new NotificationRecipient() {LastName = "3", Phone = "12345", Email = "abc@foo2.com"},
                new NotificationRecipient() {LastName = "4", Phone = "123", Email = "abc@foo.com"},
                new NotificationRecipient() {LastName = "5", Phone = "2345", Email = "abc@foo.com"}
            };

            contacts.EnsureUniqueNotifications();

            foreach(var contact in contacts)
            {
                Console.WriteLine($"{contact.LastName} {contact.Phone} {contact.Email}");
            }
        }
    }
}
