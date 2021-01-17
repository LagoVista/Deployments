using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admin.Services;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core;
using LagoVista.UserAdmin.Interfaces.Managers;

namespace LagoVista.IoT.Deployment.Tests.Host
{
    [TestClass]
    public class HostTests
    {

        DeploymentHost _host;

        Mock<ISecurity> _security = new Mock<ISecurity>();
        Mock<IUserManager> _userManager = new Mock<IUserManager>();
        Mock<IDeploymentHostRepo> _hostRepo = new Mock<IDeploymentHostRepo>();
        IDeploymentHostManager _deploymentHostManager;
        EntityHeader _org = new EntityHeader();
        EntityHeader _user = new EntityHeader();

        [TestInitialize]
        public void Init()
        {
            _host = new DeploymentHost()
            {
                Id = "MYHOSTID",
                Status = EntityHeader<HostStatus>.Create(HostStatus.Offline),
                OwnerOrganization = _org
            };
            _hostRepo.Setup(hrs => hrs.GetDeploymentHostAsync(_host.Id, true)).ReturnsAsync(_host);

            _deploymentHostManager = new DeploymentHostManager(_hostRepo.Object, new Mock<IDeploymentActivityQueueManager>().Object, new Mock<IDeploymentActivityRepo>().Object,
                new Mock<IDeploymentConnectorService>().Object, new Mock<IDeploymentInstanceRepo>().Object, new Mock<IAdminLogger>().Object, 
                new Mock<IDeploymentHostStatusRepo>().Object, _userManager.Object, new Mock<IAppConfig>().Object,
               new Mock<IDependencyManager>().Object, _security.Object);
        }

        [TestMethod]
        public void Host_ShouldUpdate_State_IfStatusIfChanged()
        {
            _host.Status = EntityHeader<HostStatus>.Create(HostStatus.Offline);

            _deploymentHostManager.UpdateDeploymentHostStatusAsync(_host.Id, Admin.Models.HostStatus.Running,"1.2.3.4", _org, _user);

            _hostRepo.Verify(hsr => hsr.UpdateDeploymentHostAsync(It.Is<DeploymentHost>(hst => hst.Status.Value == HostStatus.Running)), Times.Once);
        }

        [TestMethod]
        public void Host_ShouldUpdate_Status_DateIfChangeed()
        {
            _host.Status = EntityHeader<HostStatus>.Create(HostStatus.Offline);
            _host.StatusTimeStamp = DateTime.UtcNow.AddHours(-5).ToJSONString();
            _deploymentHostManager.UpdateDeploymentHostStatusAsync(_host.Id, Admin.Models.HostStatus.Running, "1.2.3.4", _org, _user);

            _hostRepo.Verify(hsr => hsr.UpdateDeploymentHostAsync(It.Is<DeploymentHost>(hst => hst.StatusTimeStamp.ToDateTime() > DateTime.UtcNow.AddMinutes(-5))), Times.Once);
        }

        [TestMethod]
        public void Host_Should_Not_UpdateStatus_IfStatusDidNotChange()
        {
            _host.Status = EntityHeader<HostStatus>.Create(HostStatus.Running);

            _deploymentHostManager.UpdateDeploymentHostStatusAsync(_host.Id, Admin.Models.HostStatus.Running, "1.2.3.4", _org, _user);

            _hostRepo.Verify(hsr => hsr.UpdateDeploymentHostAsync(It.Is<DeploymentHost>(hst => hst.Status.Value == HostStatus.Running)), Times.Never);
        }
    }
}