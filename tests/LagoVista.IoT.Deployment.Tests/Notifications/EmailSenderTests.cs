using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public async Task PreparePageTest()
        {
            var result = await _emailSender.PrepareMessage(GetNotification(), GetTestMode(), GetDevice(), GetLocation());
            Assert.IsTrue(result.Successful, result.ErrorMessage);
        }

        [TestMethod]
        public async Task SendTest()
        {
            var result = await _emailSender.PrepareMessage(GetNotification(), GetTestMode(), GetDevice(), GetLocation());
            Assert.IsTrue(result.Successful, result.ErrorMessage);

            result = await _emailSender.SendAsync("dontcare", GetNotification(), GetRecipient(), false, GetLinks(), OrgEH, UserEH);
            Assert.IsTrue(result.Successful, result.ErrorMessage);
        }
    }
}
