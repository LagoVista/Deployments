// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 196d429133592fb1a17577edf4614966124c7882741d2ff902d59f7a7176b1bb
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Deployment.Models.DeviceNotifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            _cotSender = new COTSender(MediaServicesManager.Object, SecureStorege.Object, AdminLogger, TagReplacer.Object, OrgMananager.Object);
        }

        [TestMethod]
        public void LoadCerts()
        {
            var certText = System.IO.File.ReadAllText("slroot.crt");
            

            var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(certText);
            var cert = new X509Certificate(bytes);


        }


        [TestMethod]
        public async Task SendAsync()
        {
            var cotMessage = new CursorOnTarget()
            {
                DataPackageFile = new Core.Models.EntityHeader()
                {
                    Id = USER_FILE_ID
                },
                UseCustomCertificate = true,
                CustomRootCert = new Core.Models.EntityHeader()
                {
                    Id = ROOT_CERT_ID
                },
            };

            var result = await _cotSender.SendAsync(cotMessage, GetDevice(), GetLocation(), OrgEH, UserEH);
            Assert.IsTrue(result.Successful, result.ErrorMessage);
        }
    }
}
