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
            _tagReplacer = new TagReplacementService(AppUserRepo.Object, SecureLinkManager.Object, AppConfig.Object, new Mock<IAdminLogger>().Object);
        }

        [TestMethod]
        public async Task ReplaceTags()
        {
            var templates = "TeST";

            await _tagReplacer.ReplaceTagsAsync(templates, false, GetDevice(), GetLocation());
        }
    }
}
