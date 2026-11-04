using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Deployment.Admin.Models;
using LagoVista.IoT.Deployment.CloudRepos.Repos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.CloudRepo.Tests
{
    [TestClass]
    public class DeploymentActivityCassandraHistoryTests
    {
        private static DeploymentActivity Create(DeploymentActivityStatus status)
        {
            return new DeploymentActivity(DeploymentActivityResourceTypes.Instance, "instance-101",
                DeploymentActivityTaskTypes.ReloadSolution)
            {
                RowKey = "activity-101",
                Status = status,
                RequestedByOrganizationId = "organization-42",
                RequestedByOrganizationName = "Organization",
                StartTimeStamp = "2026-11-04T13:00:00.000Z",
                EndTimeStamp = "2026-11-04T13:01:00.000Z",
                DurationMS = 60000,
                ErrorMessage = status == DeploymentActivityStatus.Failed ? "test failure" : null
            };
        }

        [TestMethod]
        public async Task CompletedHistoryStoresImmutableProjection()
        {
            var store = new Mock<IActivityRecordStore<CompletedDeploymentActivityRecord>>();
            CompletedDeploymentActivityRecord captured = null;
            store.Setup(x => x.InsertAsync(It.IsAny<CompletedDeploymentActivityRecord>(), It.IsAny<CancellationToken>()))
                .Callback<CompletedDeploymentActivityRecord, CancellationToken>((record, _) => captured = record)
                .Returns(Task.CompletedTask);

            await new CompletedDeploymentActivityRepo(store.Object)
                .AddCompletedDeploymentActivitiesAsync(Create(DeploymentActivityStatus.Completed));

            Assert.IsNotNull(captured);
            Assert.AreEqual("activity-101", captured.Id);
            Assert.AreEqual("instance-101", captured.ResourceId);
            Assert.AreEqual("organization-42", captured.OrganizationId);
            Assert.AreEqual(DeploymentActivityStatus.Completed.ToString(), captured.Status);
            Assert.AreEqual(DateTimeKind.Utc, captured.CreationDate.Kind);
        }

        [TestMethod]
        public async Task FailedHistoryReadsResourcePartitionAndPreservesError()
        {
            var activity = Create(DeploymentActivityStatus.Failed);
            var record = FailedDeploymentActivityRecord.From(activity);
            HistoryQuery<FailedDeploymentActivityRecord> query = null;
            var store = new Mock<IActivityRecordStore<FailedDeploymentActivityRecord>>();
            store.Setup(x => x.QueryAsync(It.IsAny<HistoryQuery<FailedDeploymentActivityRecord>>(), It.IsAny<CancellationToken>()))
                .Callback<HistoryQuery<FailedDeploymentActivityRecord>, CancellationToken>((q, _) => query = q)
                .ReturnsAsync(new StoragePageResult<FailedDeploymentActivityRecord>(new[] { record }));

            var summaries = (await new FailedDeploymentActivityRepo(store.Object)
                .GetFailedDeploymentActivitiesForResourceIdAsync("instance-101")).ToArray();

            Assert.AreEqual(1, summaries.Length);
            Assert.AreEqual("test failure", summaries[0].ErrorMessage);
            Assert.AreEqual("activity-101", summaries[0].Id);
            Assert.IsTrue(query.Filters.Any(x =>
                x.Field == nameof(FailedDeploymentActivityRecord.ResourceId) &&
                Equals(x.Value, "instance-101") && x.Operator == StorageFilterOperator.Equal));
        }
    }
}
