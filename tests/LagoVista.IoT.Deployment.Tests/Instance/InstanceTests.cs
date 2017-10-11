using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.DeviceManagement.Core.Managers;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using LagoVista.Core;
using System.Threading.Tasks;
using LagoVista.Core.Exceptions;

namespace LagoVista.IoT.Deployment.Tests.Instance
{
    /// <summary>
    /// When an instance is added/updated it will have a Device Repostiory assigned to it.  A device repository
    /// can only be assigned to once instance at a time, so this test ensure that when an instance is saved/updated
    /// 1) It must have a device repo
    /// 2) The device repo that was assigned is not in use
    /// 3) The device repo that was reassigned is not in use
    /// 4) If a device repo is swapped the original repo is reset so it can be used somewhere else.
    /// </summary>
    [TestClass]
    public class InstanceTests
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
        Mock<IDependencyManager> _dependencyManager;
        Mock<ISecurity> _security;
        Mock<IAppConfig> _appConfig;

        [TestInitialize]
        public void Init()
        {

            _deploymentHostManager = new Mock<IDeploymentHostManager>();
            _dependencyManager = new Mock<IDependencyManager>();
            _deploymentInstanceRepo = new Mock<IDeploymentInstanceRepo>();
            _deviceRepoManager = new Mock<IDeviceRepositoryManager>();
            _adminLogger = new Mock<IAdminLogger>();
            _security = new Mock<ISecurity>();
            _appConfig = new Mock<IAppConfig>();

            _instanceManager = new DeploymentInstanceManagerCore(_deploymentHostManager.Object, _deploymentInstanceRepo.Object, _deviceRepoManager.Object, _adminLogger.Object, _appConfig.Object, _dependencyManager.Object, _security.Object);

            _deploymentHostManager.Setup(dhm => dhm.GetDeploymentHostAsync(It.IsAny<string>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
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
            instance.Host = new EntityHeader<DeploymentHost>() { Id = "123", Text = "abc" };
            instance.Subscription = EntityHeader.Create("id", "text");
            instance.ContainerRepository = EntityHeader.Create("id", "text");
            instance.ContainerTag = EntityHeader.Create("id", "text");
            instance.Solution = new EntityHeader<Solution>() { Id = "id", Text = "text" };
            instance.Size = EntityHeader.Create("id", "text");
            instance.DeviceRepository = new EntityHeader<DeviceRepository>() { Id = NEW_DEVICE_REPO_ID, Text = "Don't Care" };

            return instance;
        }

        [TestMethod]
        public async Task Instance_Add_Can_Add_If_Device_Repo_Not_InUse()
        {
            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Instance = null,
                });
            });

            var instance = GetInstance();

            await _instanceManager.AddInstanceAsync(instance, null, null);

            _deviceRepoManager.Verify(dir => dir.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);
            _deploymentHostManager.Verify(dir => dir.AddDeploymentHostAsync(It.IsAny<DeploymentHost>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);
            _deploymentInstanceRepo.Verify(dir => dir.AddInstanceAsync(It.IsAny<DeploymentInstance>()), Times.Once);
            _deviceRepoManager.Verify(drm => drm.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(repo => repo.Instance.Id == DEVICE_INSTANCE_ID), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Instance_Add_ThrowValidationException_IfDeviceRepoNotAssigned()
        {

            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Instance = null
                });
            });

            var instance = GetInstance();
            instance.DeviceRepository = null;
            await _instanceManager.AddInstanceAsync(instance, null, null);
        }


        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Instance_Add_ThrowValidationException_Device_Repo_Is_InUse()
        {

            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Instance = EntityHeader.Create("id", "text"),
                });
            });

            var instance = GetInstance();

            await _instanceManager.AddInstanceAsync(instance, null, null);
        }

        [TestMethod]
        public async Task Instance_Add_Does_Not_Call_Add_If_Repo_Is_InUse()
        {
            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Instance = EntityHeader.Create("id", "text"),
                });
            });

            var instance = GetInstance();

            try
            {
                await _instanceManager.AddInstanceAsync(instance, null, null);
            }
            catch (ValidationException) { }

            _deviceRepoManager.Verify(dir => dir.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);
            _deploymentHostManager.Verify(dir => dir.AddDeploymentHostAsync(It.IsAny<DeploymentHost>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);
            _deploymentInstanceRepo.Verify(dir => dir.AddInstanceAsync(It.IsAny<DeploymentInstance>()), Times.Never);
        }

        [TestMethod]
        public async Task Instance_Update_NoDeviceRepoChange()
        {
            _deploymentInstanceRepo.Setup(inst => inst.GetInstanceAsync(DEVICE_INSTANCE_ID)).Returns((string id) =>
            {
                return Task.FromResult<DeploymentInstance>(GetInstance());
            });

            var instance = GetInstance();

            await _instanceManager.UpdateInstanceAsync(instance, null, null);


            _deploymentInstanceRepo.Verify(dir => dir.UpdateInstanceAsync(It.IsAny<DeploymentInstance>()), Times.Once);

            /* Hosting atts, didn't change, so don't update the host */
            _deploymentHostManager.Verify(dir => dir.UpdateDeploymentHostAsync(It.IsAny<DeploymentHost>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);

            /* Didn't change, don't call method to get or update the device repo */
            _deviceRepoManager.Verify(dir => dir.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);
            _deviceRepoManager.Verify(drm => drm.UpdateDeviceRepositoryAsync(It.IsAny<DeviceRepository>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);
        }

        [TestMethod]
        public async Task Instance_Update_DeviceRepoChange_NewDeviceRepoNotInUse()
        {
            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(OLD_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Id = OLD_DEVICE_REPO_ID,
                    Instance = new EntityHeader() { Id = DEVICE_INSTANCE_ID, Text = "dontcare" },
                });
            });

            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Id = NEW_DEVICE_REPO_ID,
                    Instance = null,
                });
            });

            _deploymentInstanceRepo.Setup(inst => inst.GetInstanceAsync(DEVICE_INSTANCE_ID)).Returns((string id) =>
            {
                var existingIntance = GetInstance();
                existingIntance.DeviceRepository.Id = OLD_DEVICE_REPO_ID;
                return Task.FromResult<DeploymentInstance>(existingIntance);
            });

            var updatedInstance = GetInstance();
            updatedInstance.DeviceRepository.Id = NEW_DEVICE_REPO_ID;
            await _instanceManager.UpdateInstanceAsync(updatedInstance, null, null);


            _deploymentInstanceRepo.Verify(dir => dir.UpdateInstanceAsync(It.IsAny<DeploymentInstance>()), Times.Once);

            /* Hosting atts, didn't change, so don't update the host */
            _deploymentHostManager.Verify(dir => dir.UpdateDeploymentHostAsync(It.IsAny<DeploymentHost>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);

            /* Didn't change, don't call method to get or update the device repo */
            _deviceRepoManager.Verify(dir => dir.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);

            /* Should update the new repo with the current instance */
            _deviceRepoManager.Verify(drm => drm.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rpo=>rpo.Id == NEW_DEVICE_REPO_ID && rpo.Instance.Id == DEVICE_INSTANCE_ID), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);

            /* Should clear the instance on the old repo */
            _deviceRepoManager.Verify(drm => drm.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rpo => rpo.Id == OLD_DEVICE_REPO_ID && rpo.Instance == null), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);
        }


        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public async Task Instance_Update_DeviceRepoChange_NewDeviceRepoInUse_WillThrowException()
        {
            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(OLD_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Id = OLD_DEVICE_REPO_ID,
                    Instance = new EntityHeader() { Id = DEVICE_INSTANCE_ID, Text = "dontcare" },
                });
            });

            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Id = NEW_DEVICE_REPO_ID,
                    Instance = new EntityHeader() { Id = DEVICE_INSTANCE_OTHER_ID, Text = "dontcare" },
                });
            });

            _deploymentInstanceRepo.Setup(inst => inst.GetInstanceAsync(DEVICE_INSTANCE_ID)).Returns((string id) =>
            {
                var existingIntance = GetInstance();
                existingIntance.DeviceRepository.Id = OLD_DEVICE_REPO_ID;
                return Task.FromResult<DeploymentInstance>(existingIntance);
            });

            var updatedInstance = GetInstance();
            updatedInstance.DeviceRepository.Id = NEW_DEVICE_REPO_ID;
            await _instanceManager.UpdateInstanceAsync(updatedInstance, null, null);
        }

        [TestMethod]
        public async Task Instance_Update_DeviceRepoChange_NewDeviceRepoInUse_WillNotCallUpdatesn()
        {
            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(OLD_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Id = OLD_DEVICE_REPO_ID,
                    Instance = new EntityHeader() { Id = DEVICE_INSTANCE_ID, Text = "dontcare" },
                });
            });

            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Id = NEW_DEVICE_REPO_ID,
                    Instance = new EntityHeader() { Id = DEVICE_INSTANCE_OTHER_ID, Text = "dontcare" },
                });
            });

            _deploymentInstanceRepo.Setup(inst => inst.GetInstanceAsync(DEVICE_INSTANCE_ID)).Returns((string id) =>
            {
                var existingIntance = GetInstance();
                existingIntance.DeviceRepository.Id = OLD_DEVICE_REPO_ID;
                return Task.FromResult<DeploymentInstance>(existingIntance);
            });

            var updatedInstance = GetInstance();
            updatedInstance.DeviceRepository.Id = NEW_DEVICE_REPO_ID;

            try
            {
                await _instanceManager.UpdateInstanceAsync(updatedInstance, null, null);
            }
            catch (ValidationException) { }


            _deploymentInstanceRepo.Verify(dir => dir.UpdateInstanceAsync(It.IsAny<DeploymentInstance>()), Times.Never);

            /* Hosting atts, didn't change, so don't update the host */
            _deploymentHostManager.Verify(dir => dir.UpdateDeploymentHostAsync(It.IsAny<DeploymentHost>(), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);

            /* Didn't change, don't call method to get or update the device repo */
            _deviceRepoManager.Verify(dir => dir.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);

            /* Should update the new repo with the current instance */
            _deviceRepoManager.Verify(drm => drm.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rpo => rpo.Id == NEW_DEVICE_REPO_ID && rpo.Instance.Id == DEVICE_INSTANCE_ID), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);

            /* Should clear the instance on the old repo */
            _deviceRepoManager.Verify(drm => drm.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rpo => rpo.Id == OLD_DEVICE_REPO_ID && rpo.Instance == null), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Never);
        }

        [TestMethod]
        public async Task Instance_Delete_ClearRepo()
        {
            _deviceRepoManager.Setup(drm => drm.GetDeviceRepositoryAsync(NEW_DEVICE_REPO_ID, It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>())).Returns((string id, EntityHeader user, EntityHeader org) =>
            {
                return Task.FromResult<DeviceRepository>(new DeviceRepository()
                {
                    Id = NEW_DEVICE_REPO_ID,
                    Instance = new EntityHeader() { Id = DEVICE_INSTANCE_ID, Text = "dontcare" },
                });
            });

            _deploymentInstanceRepo.Setup(inst => inst.GetInstanceAsync(DEVICE_INSTANCE_ID)).Returns((string id) =>
            {
                return Task.FromResult<DeploymentInstance>(GetInstance());
            });

            var instance = GetInstance();

            await _instanceManager.DeleteInstanceAsync(instance.Id, null, null);
        

            _deploymentInstanceRepo.Verify(dir => dir.DeleteInstanceAsync(DEVICE_INSTANCE_ID), Times.Once);
            
            /* Didn't change, don't call method to get or update the device repo */
            _deviceRepoManager.Verify(drm => drm.UpdateDeviceRepositoryAsync(It.Is<DeviceRepository>(rpo => rpo.Id == NEW_DEVICE_REPO_ID && rpo.Instance == null), It.IsAny<EntityHeader>(), It.IsAny<EntityHeader>()), Times.Once);
        }
    }
}
