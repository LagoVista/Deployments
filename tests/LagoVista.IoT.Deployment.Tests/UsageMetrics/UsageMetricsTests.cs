using Microsoft.VisualStudio.TestTools.UnitTesting;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using LagoVista.Core;

namespace LagoVista.IoT.Deployment.Tests
{
    [TestClass]
    public class UsageMetricsTests
    {
        [TestMethod]
        public void UsageMetrics_Concat()
        {
            var childMetricsmetric = new UsageMetrics();
            childMetricsmetric.BytesProcessed = 12345;
            childMetricsmetric.DeadLetterCount = 25;
            childMetricsmetric.ErrorCount = 5;
            childMetricsmetric.WarningCount = 12;
            childMetricsmetric.ProcessingMS = 1235;
            childMetricsmetric.MessagesProcessed = 1235;
            childMetricsmetric.ActiveCount = 19;

            var rolledUpMetrics = new UsageMetrics();
            rolledUpMetrics.BytesProcessed = 500;
            rolledUpMetrics.DeadLetterCount = 20;
            rolledUpMetrics.ErrorCount = 38;
            rolledUpMetrics.MessagesProcessed = 20;
            rolledUpMetrics.ProcessingMS = 123123;
            rolledUpMetrics.ActiveCount = 38;
            rolledUpMetrics.WarningCount = 15;

            rolledUpMetrics.Concat(childMetricsmetric);

            Assert.AreEqual(childMetricsmetric.BytesProcessed + 500, rolledUpMetrics.BytesProcessed);
            Assert.AreEqual(childMetricsmetric.DeadLetterCount + 20, rolledUpMetrics.DeadLetterCount);
            Assert.AreEqual(childMetricsmetric.ErrorCount + 38, rolledUpMetrics.ErrorCount);
            Assert.AreEqual(childMetricsmetric.MessagesProcessed + 20, rolledUpMetrics.MessagesProcessed);
            Assert.AreEqual(childMetricsmetric.ProcessingMS + 123123, rolledUpMetrics.ProcessingMS);
            Assert.AreEqual(childMetricsmetric.WarningCount + 15, rolledUpMetrics.WarningCount);
            Assert.AreEqual(childMetricsmetric.ActiveCount + 38, rolledUpMetrics.ActiveCount);
        }

        [TestMethod]
        public void UsageMetrics_Clone()
        {
            var metricToClone = new UsageMetrics();
            metricToClone.Version = "1.2.3.4";
            metricToClone.HostId = "hostid";
            metricToClone.InstanceId = "instanceid";
            metricToClone.PipelineModuleId = "piplinemoduleid";

            metricToClone.BytesProcessed = 12345;
            metricToClone.DeadLetterCount = 25;
            metricToClone.ErrorCount = 5;
            metricToClone.WarningCount = 12;
            metricToClone.ProcessingMS = 1235;
            metricToClone.MessagesProcessed = 1235;
            metricToClone.ActiveCount = 19;

            var clonedMetric = metricToClone.Clone();

            Assert.IsTrue(String.IsNullOrEmpty(clonedMetric.PartitionKey));
            Assert.IsTrue(String.IsNullOrEmpty(clonedMetric.RowKey));
            Assert.IsTrue(String.IsNullOrEmpty(clonedMetric.StartTimeStamp));
            Assert.IsTrue(String.IsNullOrEmpty(clonedMetric.EndTimeStamp));

            Assert.AreEqual("1.2.3.4", clonedMetric.Version);
            Assert.AreEqual("hostid", clonedMetric.HostId);
            Assert.AreEqual("instanceid", clonedMetric.InstanceId);
            Assert.AreEqual("piplinemoduleid", clonedMetric.PipelineModuleId);
            Assert.AreEqual(1235, clonedMetric.MessagesProcessed);
            Assert.AreEqual(5, clonedMetric.ErrorCount);
            Assert.AreEqual(12345, clonedMetric.BytesProcessed);
            Assert.AreEqual(12, clonedMetric.WarningCount);
            Assert.AreEqual(1235, clonedMetric.ProcessingMS);
            Assert.AreEqual(25, clonedMetric.DeadLetterCount);
            Assert.AreEqual(19, clonedMetric.ActiveCount);            
        }

        [TestMethod]
        public void UsageMetrics_Calculate_MessagePerSecond()
        {
            var childMetricsmetric = new UsageMetrics();
            var end = DateTime.Now;
            childMetricsmetric.EndTimeStamp = end.ToJSONString();
            childMetricsmetric.StartTimeStamp = end.AddMinutes(-1).ToJSONString();
            childMetricsmetric.BytesProcessed = 12345;

            childMetricsmetric.ElapsedMS = 60 * 1000;
            childMetricsmetric.ProcessingMS = 1235;
            childMetricsmetric.MessagesProcessed = 120;

            childMetricsmetric.Calculate();

            Assert.AreEqual(2, childMetricsmetric.MessagesPerSecond);
        }

        [TestMethod]
        public void UsageMetrics_Calculate_ElapsedMS()
        {
            var childMetricsmetric = new UsageMetrics();
            var end = DateTime.Now;
            childMetricsmetric.EndTimeStamp = end.ToJSONString();
            childMetricsmetric.StartTimeStamp = end.AddMinutes(-1).ToJSONString();

            childMetricsmetric.Calculate();

            Assert.AreEqual(60 * 1000, childMetricsmetric.ElapsedMS, 0.1);
        }


        [TestMethod]
        public void UsageMetrics_Calculate_AvergeProcessing()
        {
            var childMetricsmetric = new UsageMetrics();
            var end = DateTime.Now;
            childMetricsmetric.EndTimeStamp = end.ToJSONString();
            childMetricsmetric.StartTimeStamp = end.AddMinutes(-1).ToJSONString();
            childMetricsmetric.BytesProcessed = 12345;

            childMetricsmetric.ProcessingMS = 2400;
            childMetricsmetric.MessagesProcessed = 120;

            childMetricsmetric.Calculate();

            Assert.AreEqual(20, childMetricsmetric.AvergeProcessingMs);
        }
    }
}
