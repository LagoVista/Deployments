using LagoVista.Core.Models;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.CloudRepos;
using LagoVista.IoT.Deployment.CloudRepos.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using LagoVista.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepo.Tests
{
    /* Make sure our query methods work on cloud storage for scheduled activities */
    [TestClass]
    public class DeploymentActivityRepoTests
    {
        Mock<IDeploymentRepoSettings> _deploymentSettings = new Mock<IDeploymentRepoSettings>();
        Mock<IAdminLogger> _adminLogger = new Mock<IAdminLogger>();

        [TestMethod]
        public async Task ShouldNotGetFutureScheduledActivities()
        {
            _deploymentSettings.Setup(dep => dep.DeploymentAdminTableStorage).Returns(new ConnectionSettings()
            {
                AccountId = Environment.GetEnvironmentVariable("TEST_AZURESTORAGE_ACCOUNTID", EnvironmentVariableTarget.User),
                AccessKey = Environment.GetEnvironmentVariable("TEST_AZURESTORAGE_ACCESSKEY", EnvironmentVariableTarget.User)
            });

            var repo = new DeploymentActivityRepo(_deploymentSettings.Object, _adminLogger.Object);

            var activity = new DeploymentActivity(DeploymentActivityResourceTypes.Instance, "someid", DeploymentActivityTaskTypes.ReloadSolution)
            {
                RowKey = Guid.NewGuid().ToId(),
                Status = DeploymentActivityStatus.Scheduled,
                ScheduledFor = DateTime.UtcNow.AddSeconds(2).ToJSONString()
            };

            await repo.AddDeploymentActivityAsync(activity);
            var scheduledActivities = await repo.GetScheduledDeploymentActivitiesAsync();
            Assert.AreEqual(0, scheduledActivities.Where(act => act.RowKey == activity.RowKey).Count());
            await repo.RemoveDeploymentActivityAsync(activity);
        }

        [TestMethod]
        public async Task ShouldGetScheduledActivitiesIfScheudleExpires()
        {
            _deploymentSettings.Setup(dep => dep.DeploymentAdminTableStorage).Returns(new ConnectionSettings()
            {
                AccountId = Environment.GetEnvironmentVariable("TEST_AZURESTORAGE_ACCOUNTID", EnvironmentVariableTarget.User),
                AccessKey = Environment.GetEnvironmentVariable("TEST_AZURESTORAGE_ACCESSKEY", EnvironmentVariableTarget.User)
            });

            var repo = new DeploymentActivityRepo(_deploymentSettings.Object, _adminLogger.Object);

            var activity = new DeploymentActivity(DeploymentActivityResourceTypes.Instance, "someid", DeploymentActivityTaskTypes.ReloadSolution)
            {
                RowKey = Guid.NewGuid().ToId(),
                Status = DeploymentActivityStatus.Scheduled,
                ScheduledFor = DateTime.UtcNow.AddSeconds(-3).ToJSONString()
            };

            await repo.AddDeploymentActivityAsync(activity);
            var scheduledActivities = await repo.GetScheduledDeploymentActivitiesAsync();
            Assert.AreEqual(1, scheduledActivities.Where(act => act.RowKey == activity.RowKey).Count());
            await repo.RemoveDeploymentActivityAsync(activity);
        }

    }
}
