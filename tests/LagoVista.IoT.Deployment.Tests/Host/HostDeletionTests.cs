// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 89d18dee3eb7a5c1cf8b3ab04368334615592c002874953d70db689bd7d3b188
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Host
{
    [TestClass]
    public class HostDeletionTests
    {
        Mock<IUserManager> _userManager = new Mock<IUserManager>();
        Mock<ISecurity> _security = new Mock<ISecurity>();
        Mock<IDeploymentHostRepo> _hostRepo = new Mock<IDeploymentHostRepo>();
        Mock<ISecureStorage> _secureStorage = new Mock<ISecureStorage>();
        IDeploymentHostManager _deploymentHostManager;
        EntityHeader _org = new EntityHeader();
        EntityHeader _user = new EntityHeader();
        DeploymentHost _host;

        [TestInitialize]
        public void HostDelete_CanNotDeleteMCP()
        {
            _host = new DeploymentHost()
            {
                Id = "MYHOSTID",
                Status = EntityHeader<HostStatus>.Create(HostStatus.Offline),
                HostType = EntityHeader<HostTypes>.Create(HostTypes.MCP)
            };
            _hostRepo.Setup(hrs => hrs.GetDeploymentHostAsync(_host.Id, true)).ReturnsAsync(_host);


            _deploymentHostManager = new DeploymentHostManager(_hostRepo.Object, new Mock<IDeploymentActivityQueueManager>().Object, new Mock<IDeploymentActivityRepo>().Object,
                    new Mock<IDeploymentConnectorService>().Object, new Mock<ISecureStorage>().Object, new Mock<IDeploymentInstanceRepo>().Object, new Mock<IAdminLogger>().Object,
                    new Mock<IDeploymentHostStatusRepo>().Object, _userManager.Object, new Mock<IAppConfig>().Object,
                    new Mock<IDependencyManager>().Object, _security.Object);
        }

        [TestMethod]
        public async Task HostDelete_DeleteShoudlFailForNotificationserver()
        {
            _host.HostType = EntityHeader<HostTypes>.Create(HostTypes.Notification);
            var result = await _deploymentHostManager.DeleteDeploymentHostAsync(_host.Id, _org, _user);
            Assert.IsFalse(result.Successful);
        }


        [TestMethod]
        public async Task HostDelete_DeleteShouldFailForMCPServer()
        {
            _host.HostType = EntityHeader<HostTypes>.Create(HostTypes.MCP);
            var result = await _deploymentHostManager.DeleteDeploymentHostAsync(_host.Id, _org, _user);
            Assert.IsFalse(result.Successful);
        }


        [TestMethod]
        public async Task HostDelete_DeleteNotificationShouldReturnErrHST1001()
        {
            _host.HostType = EntityHeader<HostTypes>.Create(HostTypes.MCP);
            var result = await _deploymentHostManager.DeleteDeploymentHostAsync(_host.Id, _org, _user);
            Assert.AreEqual(LagoVista.IoT.Deployment.Admin.Resources.DeploymentErrorCodes.CanNotDeleteMCPHost.Code, result.Errors.First().ErrorCode);
        }

        [TestMethod]
        public async Task HostDelete_DeleteShouldFailForMCPServerErrHST1002()
        {
            _host.HostType = EntityHeader<HostTypes>.Create(HostTypes.Notification);
            var result = await _deploymentHostManager.DeleteDeploymentHostAsync(_host.Id, _org, _user);
            Assert.AreEqual(LagoVista.IoT.Deployment.Admin.Resources.DeploymentErrorCodes.CanNotDeleteNotificationServerHost.Code, result.Errors.First().ErrorCode);
        }
    }
}
