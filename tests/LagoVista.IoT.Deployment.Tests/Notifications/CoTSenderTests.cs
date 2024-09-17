using LagoVista.Core;
using LagoVista.IoT.Deployment.Admin.Interfaces;
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
    public class CoTSenderTests : TestBase
    {
        ICOTSender _cotSender;

        [TestInitialize]
        public void Init()
        {
            InitBase();
            _cotSender = new COTSender(MediaServicesManager.Object, AdminLogger, TagReplacer.Object);
        }


        [TestMethod]
        public async Task SendAsync()
        {
            var cotMessage = new CursorOnTarget()
            {
                DataPackageFile = new Core.Models.EntityHeader()
                {
                    Id = Guid.NewGuid().ToId()
                }
            };

            var result = await _cotSender.SendAsync(cotMessage, GetDevice(), GetLocation(), OrgEH, UserEH);
            Assert.IsTrue(result.Successful, result.ErrorMessage);
        }
    }
}
