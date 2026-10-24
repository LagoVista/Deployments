// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ba07cb4fb85ed55ba1764b327c5bc6d05da6fca695cdc5c436923a4d73ff4944
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Deployment.Models;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
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
    public class NotificationSenderTests :  TestBase
    {

        INotificationSender _sender;
        private Mock<IDeviceNotificationRecipientRepo> _recipientRepo = new Mock<IDeviceNotificationRecipientRepo>();

        [TestInitialize]
        public void Init()
        {
            _sender = new NotificationSender(AdminLogger, DistroLibRepo.Object, NotificationTacker.Object, NotificationRepo.Object, OrgLocationRepo.Object, DeviceManager.Object, EmailSender.Object, SMSSender.Object, LandingPageBuilder.Object, OrgRepo.Object, _recipientRepo.Object,
                                RasiedDeviceNotificationRepo.Object, NotificationPublisher.Object, BackgroundTaskQueue.Object, TagReplacer.Object,  DeviceConfigHelper.Object, AppUserRepo.Object, RepoManager.Object, COTSender.Object, RestSender.Object, MqttSender.Object, DeviceCommandSender.Object,
                                DeploymentInstanceRepo.Object, TimeZoneService.Object);
        }


        [TestMethod]
        public async Task SendNotificationAsync()
        {
            var raised = GetRaisedNotification();
            var repository = new DeviceRepository()
            {
                Id = raised.DeviceRepositoryId,
                Name = "Fixture Repository",
                OwnerOrganization = OrgEH,
                Instance = EntityHeader.Create("AABBCCDDEEFF00112233445566778899", "Fixture Deployment")
            };
            var device = GetDevice();
            device.Id = "00112233445566778899AABBCCDDEEFF";
            device.DeviceId = raised.DeviceId;
            device.Name = "Fixture Device";
            device.OwnerOrganization = OrgEH;
            device.DeviceRepository = EntityHeader.Create(repository.Id, repository.Name);
            var notification = GetNotification();
            notification.Name = "Fixture Notification";
            notification.Id = "11223344556677889900AABBCCDDEEFF";

            RepoManager.Setup(x => x.GetDeviceRepositoryWithSecretsAsync(raised.DeviceRepositoryId, OrgEH, UserEH)).ReturnsAsync(repository);
            DeviceManager.Setup(x => x.GetDeviceByDeviceIdAsync(repository, raised.DeviceId, OrgEH, UserEH))
                .ReturnsAsync(InvokeResult<Device>.Create(device));
            NotificationRepo.Setup(x => x.GetNotificationByKeyAsync(OrgEH.Id, It.IsAny<string>(), raised.NotificationKey))
                .ReturnsAsync(notification);
            var recipientRepo = Mock.Get<IDeviceNotificationRecipientRepo>(Mock.Of<IDeviceNotificationRecipientRepo>());
            recipientRepo.Setup(x => x.GetRecipientsForDeviceAndNotificationKeyAsync(It.IsAny<ListRequest>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ListResponse<DeviceNotificationRecipient> { Model = new List<DeviceNotificationRecipient>() });
            DeploymentInstanceRepo.Setup(x => x.GetReadOnlyInstanceAsync(repository.Instance.Id))
                .ReturnsAsync(new DeploymentInstance() { Id = repository.Instance.Id });
            var result = await _sender.RaiseNotificationAsync(raised, OrgEH, UserEH);
            Assert.IsTrue(result.Successful, result.ErrorMessage);
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
