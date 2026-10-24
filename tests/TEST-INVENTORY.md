# Deployments test inventory

Baseline source 69a9a0075d8bfecd935b7c75cdb983db3ae6e05b; MSTest .NET 9. Two test projects: LagoVista.IoT.Deployment.Tests and LagoVista.IoT.Deployment.CloudRepo.Tests. Source inspection counted 83 attributed test methods; runtime discovery remains unverified.

CloudRepo.Tests: DeploymentActivityRepoTests (2).

Deployment.Tests test classes (source path relative to project, attributed method counts): DeviceConfig/RouteMappings (1), WatchDogExclusionValidationTests (7); Host/HostDeletionTests (4), HostStatusTests (3), SharedHostTests (0); Instance/InstanceTests (11); Notifications/CoTSenderTests (2), DeviceErrorQueueSchedue (1), EmailSenderTests (2), LandingPageBuilderTests (1), MqttSenderTests (1), NotificationSenderTests (2), RestSenderTests (1), SMSSenderTests (2), TagReplacementTests (1), TestBase helper (0); Publishing/Deserialization (1); Tak/TakFileLoaderTests (1), XmlParserTests (6); TimeZones/TimeZoneTests (12); UsageMetrics/UsageMetricsTests (5); Validation/InputTranslatorToInputValidationTests (8), ListenerValidationValidation (1), OutputCommandToOutputTranslatorValidationTests (8), ValidationTestBase helper (0).

Provisional integration candidates: CloudRepo repository tests and notification delivery/scheduling cases. Inspect exact fixture and service dependencies before assigning categories. Provisional unit candidates: isolated model validation, host/instance logic, XML/timezone/metrics parsing. SharedHostTests has no directly attributed test methods. Open discovery questions: inherited/data-driven cases, ignored tests, runtime discovery, build errors, setup and external service availability. Do not treat this source inventory as runtime test discovery or passing test proof.

Next tasks: mark confirmed integration tests with MSTest [TestCategory("Integration")], confirm fast-unit filtering, record complete runtime discovery and baseline failures, evaluate brittle tests without deleting valuable coverage.
