using LagoVista.Core;
using LagoVista.Core.Exceptions;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.UserAdmin.Interfaces.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.Host
{
    [TestClass]
    public class SharedHostTests
    {
        private const string NEW_DEVICE_REPO_ID = "ABC123ABC123ABC123";
        private const string OLD_DEVICE_REPO_ID = "DEF123DEF321DEF123";
        private const string DEVICE_INSTANCE_ID = "INSTANCEID001";
        private const string DEVICE_INSTANCE_OTHER_ID = "INSTANCEID001_OTHER";

        IDeploymentInstanceManagerCore _instanceManager;
        Mock<IDeploymentHostManager> _deploymentHostManager;
        Mock<IDeploymentInstanceRepo> _deploymentInstanceRepo;
        Mock<IDeviceRepositoryManager> _deviceRepoManager;
        Mock<IAdminLogger> _adminLogger;
        Mock<IUserManager> _userManager = new Mock<IUserManager>();
        Mock<IDependencyManager> _dependencyManager;
        Mock<ISecureStorage> _secureStorage;
        Mock<ISecurity> _security;
        Mock<IAppConfig> _appConfig;
        Mock<IOrganizationManager> _orgManager;
        Mock<IDeploymentInstanceStatusRepo> _instanceStatusRepo = new Mock<IDeploymentInstanceStatusRepo>();


        EntityHeader ORG = EntityHeader.Create("FB4C4FD759E3497180DF1B28FFCB7ACD", "MyOrg");
        EntityHeader USER = EntityHeader.Create("{B050B8F2B72D46E8-8555BCE178111218", "MyUser");

        [TestInitialize]
        public void Init()
        {
            _deploymentHostManager = new Mock<IDeploymentHostManager>();
            _dependencyManager = new Mock<IDependencyManager>();
            _deploymentInstanceRepo = new Mock<IDeploymentInstanceRepo>();
            _deviceRepoManager = new Mock<IDeviceRepositoryManager>();
            _adminLogger = new Mock<IAdminLogger>();
            _security = new Mock<ISecurity>();
            _secureStorage = new Mock<ISecureStorage>();
            _appConfig = new Mock<IAppConfig>();
            _orgManager = new Mock<IOrganizationManager>();

            _userManager.Setup(um => um.FindByIdAsync(It.Is<string>(id => id == USER.Id))).ReturnsAsync(new UserAdmin.Models.Users.AppUser()
            {

            });

            _secureStorage.Setup(ss => ss.AddSecretAsync(It.IsAny<EntityHeader>(), It.IsAny<string>())).ReturnsAsync(InvokeResult<string>.Create("XXXX"));

            _instanceManager = new DeploymentInstanceManagerCore(_deploymentHostManager.Object, _deploymentInstanceRepo.Object, _deviceRepoManager.Object,
                _secureStorage.Object, _instanceStatusRepo.Object, _userManager.Object, _adminLogger.Object,
                _appConfig.Object, _dependencyManager.Object, _security.Object);

            _deploymentHostManager.Setup(dhm => dhm.GetDeploymentHostAsync(It.IsAny<string>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>(), It.IsAny<bool>())).Returns((string id, EntityHeader user, EntityHeader org, bool throwOnError) =>
            {
                var instance = GetInstance();

                return Task.FromResult<DeploymentHost>(new DeploymentHost()
                {
                    CloudProvider = instance.CloudProvider,
                    Size = instance.Size,
                    Subscription = instance.Subscription,
                    ContainerRepository = instance.ContainerRepository,
                    ContainerTag = instance.ContainerTag
                });
            });
        }

        private DeploymentInstance GetInstance()
        {
            var instance = new DeploymentInstance();
            instance.Id = DEVICE_INSTANCE_ID;
            instance.CreatedBy = EntityHeader.Create(Guid.NewGuid().ToId(), "username");
            instance.LastUpdatedBy = instance.CreatedBy;
            instance.CreationDate = DateTime.Now.ToJSONString();
            instance.LastUpdatedDate = DateTime.Now.ToJSONString();
            instance.Key = "abc123";
            instance.Name = "myinstance";
            instance.OwnerOrganization = ORG;
            instance.PrimaryHost = new EntityHeader<DeploymentHost>() { Id = "123", Text = "abc" };
            instance.Subscription = EntityHeader.Create("id", "text");
            instance.ContainerRepository = EntityHeader.Create("id", "text");
            instance.ContainerTag = EntityHeader.Create("id", "text");
            instance.Solution = new EntityHeader<Solution>() { Id = "id", Text = "text" };
            instance.Size = EntityHeader.Create("id", "text");
            instance.NuvIoTEdition = EntityHeader<NuvIoTEditions>.Create(NuvIoTEditions.App);
            instance.WorkingStorage = EntityHeader<WorkingStorage>.Create(WorkingStorage.Cloud);
            instance.LogStorage = EntityHeader<LogStorage>.Create(LogStorage.Cloud);
            instance.DeviceRepository = new EntityHeader<DeviceRepository>() { Id = NEW_DEVICE_REPO_ID, Text = "Don't Care" };
            instance.DeploymentConfiguration = EntityHeader<DeploymentConfigurations>.Create(DeploymentConfigurations.SingleInstance);
            instance.DeploymentType = EntityHeader<DeploymentTypes>.Create(DeploymentTypes.Managed);
            instance.QueueType = EntityHeader<QueueTypes>.Create(QueueTypes.InMemory);
            instance.PrimaryCacheType = EntityHeader<Pipeline.Models.CacheTypes>.Create(Pipeline.Models.CacheTypes.LocalInMemory);
            instance.SharedAccessKey1 = "ABC123";
            instance.SharedAccessKey2 = "ABC123";
            return instance;
        }


    }
}
