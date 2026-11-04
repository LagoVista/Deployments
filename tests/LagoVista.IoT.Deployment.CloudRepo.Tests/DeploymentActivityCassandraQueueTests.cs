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
    public class DeploymentActivityCassandraQueueTests
    {
        private static DeploymentActivity Scheduled()
        {
            return new DeploymentActivity(DeploymentActivityResourceTypes.Server, "server-1",
                DeploymentActivityTaskTypes.RestartHost)
            {
                RowKey = "queue-1",
                Status = DeploymentActivityStatus.Scheduled,
                ScheduledFor = "2026-11-04T13:00:00.000Z",
                RetryCount = 2
            };
        }

        [TestMethod]
        public void RecordRoundTripsDeploymentActivity()
        {
            var activity = Scheduled();
            var record = ActiveDeploymentActivityRecord.From(activity);
            record.Version = 3;

            var restored = record.ToActivity();

            Assert.AreEqual("queue-1", restored.RowKey);
            Assert.AreEqual("server-1", restored.PartitionKey);
            Assert.AreEqual(DeploymentActivityTaskTypes.RestartHost, restored.Type);
            Assert.AreEqual(DeploymentActivityStatus.Scheduled, restored.Status);
            Assert.AreEqual(2, restored.RetryCount);
            Assert.AreEqual(3L, restored.StorageVersion);
            Assert.AreEqual(DateTimeKind.Utc, record.ScheduledFor.Value.Kind);
        }

        [TestMethod]
        public async Task AddAndReplaceUseVersionedConditionalMutations()
        {
            var store = new Mock<IConditionalOperationalDataStore<ActiveDeploymentActivityRecord>>();
            store.Setup(x => x.TryCreateAsync(It.IsAny<ActiveDeploymentActivityRecord>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalMutationResult<ActiveDeploymentActivityRecord>(
                    true, new ActiveDeploymentActivityRecord { Version = 1 }));

            store.Setup(x => x.TryReplaceAsync(It.IsAny<ActiveDeploymentActivityRecord>(), 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalMutationResult<ActiveDeploymentActivityRecord>(
                    true, new ActiveDeploymentActivityRecord { Version = 2 }));

            var repo = new DeploymentActivityRepo(store.Object);
            var activity = Scheduled();
            await repo.AddDeploymentActivityAsync(activity);
            Assert.AreEqual(1L, activity.StorageVersion);

            activity.Status = DeploymentActivityStatus.Running;
            await repo.UpdateDeploymentActivityAsync(activity);
            Assert.AreEqual(2L, activity.StorageVersion);
            store.Verify(x => x.TryReplaceAsync(It.IsAny<ActiveDeploymentActivityRecord>(), 1,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ScheduledQueryUsesDueTimeRangeFilter()
        {
            StorageQuery<ActiveDeploymentActivityRecord> captured = null;
            var store = new Mock<IConditionalOperationalDataStore<ActiveDeploymentActivityRecord>>();
            store.Setup(x => x.QueryAsync(It.IsAny<StorageQuery<ActiveDeploymentActivityRecord>>(), It.IsAny<CancellationToken>()))
                .Callback<StorageQuery<ActiveDeploymentActivityRecord>, CancellationToken>((query, _) => captured = query)
                .ReturnsAsync(new StoragePageResult<ActiveDeploymentActivityRecord>(
                    Array.Empty<ActiveDeploymentActivityRecord>()));

            await new DeploymentActivityRepo(store.Object).GetScheduledDeploymentActivitiesAsync();

            Assert.IsTrue(captured.Filters.Any(filter =>
                filter.Field == nameof(ActiveDeploymentActivityRecord.Status) &&
                Equals(filter.Value, DeploymentActivityStatus.Scheduled.ToString())));
            Assert.IsTrue(captured.Filters.Any(filter =>
                filter.Field == nameof(ActiveDeploymentActivityRecord.ScheduledFor) &&
                filter.Operator == StorageFilterOperator.LessThan));
        }
    }
}
