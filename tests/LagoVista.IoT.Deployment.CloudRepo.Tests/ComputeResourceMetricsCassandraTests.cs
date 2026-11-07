using LagoVista.CloudStorage.Storage;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
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
    public class ComputeResourceMetricsCassandraTests
    {
        [TestMethod]
        public async Task AppendsHostSampleWithOrgAndPreservesMetricFields()
        {
            ComputeResourceMetricActivityRecord inserted = null;
            var store = new Mock<IActivityRecordStore<ComputeResourceMetricActivityRecord>>();
            store.Setup(x => x.InsertAsync(It.IsAny<ComputeResourceMetricActivityRecord>(), It.IsAny<CancellationToken>()))
                .Callback<ComputeResourceMetricActivityRecord, CancellationToken>((record, _) => inserted = record)
                .Returns(Task.CompletedTask);

            var sample = new ComputeResourceMetrics
            {
                PartitionKey = "host-100",
                RowKey = "metric-100",
                StartTimeStamp = "2026-11-07T02:00:00Z",
                EndTimeStamp = "2026-11-07T02:01:00Z",
                ElapsedMS = 60000,
                Status = "Running",
                StatusDetails = "OK",
                CpuPercent = "18%",
                MemoryPercent = "320 mb"
            };

            await new ComputeResourceMetricsRepo(store.Object)
                .AddMetricAsync(sample, EntityHeader.Create("org-1", "Organization One"));

            Assert.IsNotNull(inserted);
            Assert.AreEqual("metric-100", inserted.Id);
            Assert.AreEqual("host-100", inserted.HostId);
            Assert.AreEqual("org-1", inserted.OrganizationId);
            Assert.AreEqual(DateTimeKind.Utc, inserted.CreationDate.Kind);
            Assert.AreEqual("18%", inserted.CpuPercent);
            Assert.AreEqual("320 mb", inserted.MemoryPercent);
            Assert.AreEqual("OK", inserted.StatusDetails);
        }

        [TestMethod]
        public async Task HostHistoryQueriesHostPartitionAndPreservesSample()
        {
            HistoryQuery<ComputeResourceMetricActivityRecord> query = null;
            var sample = new ComputeResourceMetricActivityRecord
            {
                Id = "metric-100",
                HostId = "host-100",
                CreationDate = DateTime.UtcNow.AddMinutes(-5),
                CpuPercent = "18%",
                MemoryPercent = "320 mb",
                Status = "Running"
            };
            var store = new Mock<IActivityRecordStore<ComputeResourceMetricActivityRecord>>();
            store.Setup(x => x.QueryAsync(It.IsAny<HistoryQuery<ComputeResourceMetricActivityRecord>>(), It.IsAny<CancellationToken>()))
                .Callback<HistoryQuery<ComputeResourceMetricActivityRecord>, CancellationToken>((q, _) => query = q)
                .ReturnsAsync(new StoragePageResult<ComputeResourceMetricActivityRecord>(new[] { sample }));

            var result = await new ComputeResourceMetricsRepo(store.Object)
                .GetMetricsForHostAsync("host-100", ListRequest.Create(1, 20));

            Assert.IsNotNull(result);
            Assert.IsNotNull(query);
            Assert.IsTrue(query.Filters.Any(filter =>
                filter.Field == nameof(ComputeResourceMetricActivityRecord.HostId) &&
                Equals(filter.Value, "host-100") &&
                filter.Operator == StorageFilterOperator.Equal));
        }
    }
}
