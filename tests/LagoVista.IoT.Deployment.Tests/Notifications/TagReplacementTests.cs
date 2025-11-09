// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 08b8d61531f892a13f3650ab11137228033bd00abc3ede56143b2d05dc3e8273
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Services.NotificationClients;
using LagoVista.IoT.Logging.Loggers;
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
    public class TagReplacementTests : TestBase
    {
        ITagReplacementService _tagReplacer;

        [TestInitialize]
        public void Init()
        {
            _tagReplacer = new TagReplacementService(AppUserRepo.Object, SecureLinkManager.Object, new Mock<ICustomerTagReplacementService>().Object, AppConfig.Object, new Mock<IAdminLogger>().Object);
        }

        [TestMethod]
        public async Task ReplaceTags()
        {
            var templates = "TeST";

            await _tagReplacer.ReplaceTagsAsync(templates, false, GetDevice(), GetLocation());
        }
    }
}
