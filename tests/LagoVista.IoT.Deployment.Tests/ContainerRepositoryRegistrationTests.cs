using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin;
using LagoVista.IoT.Deployment.Admin.Interfaces;
using LagoVista.IoT.Deployment.Admin.Managers;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.Admin.Models.DockerSupport;
using LagoVista.IoT.Deployment.Admin.Repos;
using LagoVista.IoT.Deployment.Admin.Rest.Controllers;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.IoT.Web.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LagoVista.IoT.Deployment.Tests
{
    [TestClass]
    public class ContainerRepositoryRegistrationTests
    {
        private Mock<IContainerRepositoryRepo> _repo;
        private Mock<ISecureStorage> _secureStorage;
        private ContainerRepositoryManager _manager;

        [TestInitialize]
        public void Init()
        {
            _repo = new Mock<IContainerRepositoryRepo>();
            _secureStorage = new Mock<ISecureStorage>();
            _manager = new ContainerRepositoryManager(
                _repo.Object,
                _secureStorage.Object,
                new Mock<IAdminLogger>().Object,
                new Mock<IDockerRegisteryServices>().Object,
                new Mock<IAppConfig>().Object,
                new Mock<IDependencyManager>().Object,
                new Mock<ISecurity>().Object);
        }

        [TestMethod]
        public async Task RegisterContainerTagCreatesMissingRepositoryWithoutRegistryCredentials()
        {
            _repo.Setup(repo => repo.GetContainerRepoByNameAsync("ORG1", "nuviot", "shared-runtime"))
                .ReturnsAsync((ContainerRepository)null);
            ContainerRepository created = null;
            _repo.Setup(repo => repo.AddContainerRepoAsync(It.IsAny<ContainerRepository>()))
                .Callback<ContainerRepository>(container => created = container)
                .Returns(Task.CompletedTask);
            _repo.Setup(repo => repo.UpdateContainerRepoAsync(It.IsAny<ContainerRepository>()))
                .Returns(Task.CompletedTask);

            var tag = new TaggedContainer { Name = "v1.2.3", Key = "v1x2x3", Tag = "v1.2.3" };
            var result = await _manager.RegisterContainerTagAsync("ORG1", "nuviot/shared-runtime", tag);

            Assert.IsNotNull(created);
            Assert.AreSame(created, result);
            Assert.AreEqual("ORG1", created.OwnerOrganization.Id);
            Assert.AreEqual("nuviot", created.Namespace);
            Assert.AreEqual("shared-runtime", created.RepositoryName);
            Assert.IsTrue(String.IsNullOrWhiteSpace(created.UserName));
            Assert.IsTrue(String.IsNullOrWhiteSpace(created.SecurePasswordId));
            Assert.AreEqual(1, created.Tags.Count);
            Assert.AreEqual("v1.2.3", created.Tags[0].Tag);
            Assert.AreEqual(tag.Id.ToString(), created.PreferredTag.Id);
        }

        [TestMethod]
        public async Task RegisterContainerTagReusesRepositoryAndReplacesSameTag()
        {
            var old = new TaggedContainer { Name = "old", Key = "old", Tag = "v1.2.3" };
            var existing = new ContainerRepository
            {
                Id = "11111111111111111111111111111111",
                Name = "nuviot/shared-runtime",
                Key = "nuviot-shared-runtime",
                Namespace = "nuviot",
                RepositoryName = "shared-runtime",
                OwnerOrganization = EntityHeader.Create("ORG1", "ORG1")
            };
            existing.Tags.Add(old);
            _repo.Setup(repo => repo.GetContainerRepoByNameAsync("ORG1", "nuviot", "shared-runtime")).ReturnsAsync(existing);
            _repo.Setup(repo => repo.UpdateContainerRepoAsync(existing)).Returns(Task.CompletedTask);

            var replacement = new TaggedContainer { Name = "new", Key = "new", Tag = "v1.2.3" };
            var result = await _manager.RegisterContainerTagAsync("ORG1", "nuviot/shared-runtime", replacement);

            Assert.AreSame(existing, result);
            Assert.AreEqual(1, result.Tags.Count(tag => tag.Tag == "v1.2.3"));
            Assert.AreSame(replacement, result.Tags[0]);
            Assert.AreEqual(replacement.Id.ToString(), result.PreferredTag.Id);
            _repo.Verify(repo => repo.AddContainerRepoAsync(It.IsAny<ContainerRepository>()), Times.Never);
        }

        [TestMethod]
        public async Task RemoteRegistryLookupRequiresCredentialsOnlyWhenInvoked()
        {
            var container = new ContainerRepository
            {
                Id = "11111111111111111111111111111111",
                Namespace = "nuviot",
                RepositoryName = "shared-runtime",
                OwnerOrganization = EntityHeader.Create("ORG1", "ORG1")
            };
            _repo.Setup(repo => repo.GetContainerRepoAsync(container.Id)).ReturnsAsync(container);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _manager.GetTagsFromRemoteRegistryAsync(container.Id, EntityHeader.Create("ORG1", "ORG1"), EntityHeader.Create("USER1", "USER1")));
            _secureStorage.Verify(storage => storage.GetSecretAsync(It.IsAny<EntityHeader>(), It.IsAny<string>(), It.IsAny<EntityHeader>()), Times.Never);
        }

        [TestMethod]
        public void ServiceControllerUsesSignedOrganizationScopedRoute()
        {
            var controller = typeof(ContainerRepositoryServiceController);
            Assert.IsNotNull(controller.GetCustomAttribute<RequireSignedRequestAttribute>());
            Assert.IsNull(controller.GetCustomAttribute<AuthorizeAttribute>());
            Assert.AreEqual("api/services/{orgId}/container", controller.GetCustomAttribute<RouteAttribute>().Template);

            var method = controller.GetMethod(nameof(ContainerRepositoryServiceController.RegisterTagAsync));
            Assert.IsNotNull(method);
            Assert.AreEqual("{containerName}/tag", method.GetCustomAttribute<HttpPostAttribute>().Template);
        }
    }
}
