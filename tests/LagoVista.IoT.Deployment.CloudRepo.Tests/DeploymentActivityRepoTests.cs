using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.CloudRepos.Repos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepo.Tests
{
    [TestClass]
    public class DeploymentActivityRepoTests
    {
        [TestMethod]
        public async Task ScheduledQueryUsesCassandraDueTimeFilter()
        {
            StorageQuery<ActiveDeploymentActivityRecord> captured = null;
            var store = new Mock<IConditionalOperationalDataStore<ActiveDeploymentActivityRecord>>();
            store.Setup(x => x.QueryAsync(It.IsAny<StorageQuery<ActiveDeploymentActivityRecord>>(), It.IsAny<CancellationToken>()))
                .Callback<StorageQuery<ActiveDeploymentActivityRecord>, CancellationToken>((query, _) => captured = query)
                .ReturnsAsync(new StoragePageResult<ActiveDeploymentActivityRecord>(
                    Array.Empty<ActiveDeploymentActivityRecord>()));

            var activities = await new DeploymentActivityRepo(store.Object).GetScheduledDeploymentActivitiesAsync();

            Assert.AreEqual(0, activities.Count());
            Assert.IsTrue(captured.Filters.Any(filter =>
                filter.Field == nameof(ActiveDeploymentActivityRecord.Status) &&
                Equals(filter.Value, DeploymentActivityStatus.Scheduled.ToString())));
            Assert.IsTrue(captured.Filters.Any(filter =>
                filter.Field == nameof(ActiveDeploymentActivityRecord.ScheduledFor) &&
                filter.Operator == StorageFilterOperator.LessThan &&
                ((DateTime)filter.Value) <= DateTime.UtcNow));
        }

        [TestMethod]
        public async Task ScheduledQueryReturnsDueActivity()
        {
            var activity = new DeploymentActivity(
                DeploymentActivityResourceTypes.Instance, "resource-1",
                DeploymentActivityTaskTypes.ReloadSolution)
            {
                RowKey = "due-activity",
                Status = DeploymentActivityStatus.Scheduled,
                ScheduledFor = DateTime.UtcNow.AddMinutes(-5).ToString("O")
            };
            var record = ActiveDeploymentActivityRecord.From(activity);
            record.Version = 1;

            var store = new Mock<IConditionalOperationalDataStore<ActiveDeploymentActivityRecord>>();
            store.Setup(x => x.QueryAsync(It.IsAny<StorageQuery<ActiveDeploymentActivityRecord>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StoragePageResult<ActiveDeploymentActivityRecord>(new[] { record }));

            var result = (await new DeploymentActivityRepo(store.Object)
                .GetScheduledDeploymentActivitiesAsync()).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("due-activity", result[0].RowKey);
            Assert.AreEqual(1L, result[0].StorageVersion);
        }
    }
}
