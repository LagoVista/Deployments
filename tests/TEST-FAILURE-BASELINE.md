# Deployments exact-source failure baseline — Task 3

Repository LagoVista/Deployments; commit cb498ec1c04df47f261d9e451028916bb2ae617e.
Canonical invocation: workspace-test with semantic filter `TestCategory!=Integration`, command `f365-integration-unit-filter-test-001`.
Published evidence: softwarelogistics/quality branch `reports`, path `reports/lagovista/deployments/cb498ec1c04df47f261d9e451028916bb2ae617e/6661122fa8bba19bc90000bd8e6925384d23731f6de8d3e8751cb3e74fdf26ef/test-results/tests.json`.
Observed: 77 executed, 52 passed, 25 failed, 0 skipped, approximately 7.2783 seconds. Build succeeded. Six tagged external-service tests were excluded from standard unit execution; no integration-test pass is claimed. This baseline is for source cb498ec1, and later inventory/documentation commits do not alter executable test code.

## Failure groups (every failure covered)

**Invalid NormalizedId32 fixtures: 22 failures.** Format requires 32 uppercase ASCII letters/digits. Current tests supply invalid short names; these fail before intended behavior is evaluated.
- Host/HostDeletionTests: HostDelete_DeleteNotificationShouldReturnErrHST1001; HostDelete_DeleteShoudlFailForNotificationserver; HostDelete_DeleteShouldFailForMCPServer; HostDelete_DeleteShouldFailForMCPServerErrHST1002. All four fail in setup with `MYHOSTID`.
- Host/HostStatusTests (class HostTests): Host_ShouldUpdate_State_IfStatusIfChanged; Host_ShouldUpdate_Status_DateIfChangeed; Host_Should_Not_UpdateStatus_IfStatusDidNotChange. Three setup failures with `MYHOSTID`.
- Instance/InstanceTests: Instance_Add_Can_Add_If_Device_Repo_Not_InUse; Instance_Add_Does_Not_Call_Add_If_Repo_Is_InUse; Instance_Add_ThrowValidationException_Device_Repo_Is_InUse; Instance_Add_ThrowValidationException_IfDeviceRepoNotAssigned; Instance_Delete_ClearRepo; Instance_Delete_DeletesHostIfDedicated; Instance_Delete_ShouldNotDeleteIfNotDedicated; Instance_Update_DeviceRepoChange_NewDeviceRepoInUse_WillNotCallUpdatesn; Instance_Update_DeviceRepoChange_NewDeviceRepoInUse_WillThrowException; Instance_Update_DeviceRepoChange_NewDeviceRepoNotInUse; Instance_Update_NoDeviceRepoChange. All 11 fail on short identifiers such as `INSTANCEID001`, `MYHOSTID`, or `ABC123ABC123ABC123`.
- Validation/InputTranslatorToInputValidationTests: InputTranslator_To_Input_StateSetMisMatch_InValid; InputTranslator_To_Input_StateSet_Valid; InputTranslator_To_Input_UnitSetMisMatch_InValid; InputTranslator_To_Input_UnitSet_Valid. Four fail on `stateset1` / `unitset1`.

**NullReferenceException: 3 failures.** Investigate setup and Moq return values before changing production implementation.
- Notifications/EmailSenderTests.SendTest => `src/LagoVista.IoT.Deployment.Admin/Services/NotificationClients/EmaiLSender.cs:111`.
- Notifications/NotificationSenderTests.SendNotificationAsync => `src/LagoVista.IoT.Deployment.Admin/Services/NotificationClients/NotificationSender.cs:224`.
- Notifications/SMSSenderTests.SendTest => `src/LagoVista.IoT.Deployment.Admin/Services/NotificationClients/SMSSender.cs:101`.

## Immediate repair hypotheses for future tasks
1. Replace invalid fixture identifiers consistently with distinct valid 32-character A-Z/0-9 identifiers, preserving equality/inequality expectations. Rerun rather than assuming passing; secondary assertions may emerge.
2. Inspect three NullReference lines with test setup and mock configuration; correct fixture construction where possible. Preserve real behavior coverage.
3. Evaluate no-assertion tests (CoTSenderTests.LoadCerts and NotificationSenderTests.UniqueNotificationsTest) in Task 4, but do not remove them merely because they are weak, and note neither failed in this filtered run.

## Integration prerequisites and environmental outcomes
- CloudRepo.Tests two Azure Table Storage operations: `TEST_AZURESTORAGE_ACCOUNTID` and `TEST_AZURESTORAGE_ACCESSKEY`; writes/queries/removes storage entries.
- DeviceErrorQueueSchedule: `DEV_SB_NUVIOT_ERRS_ACCOUNT_ID`, `DEV_SB_NUVIOT_ERRS_USER_NAME`, `DEV_SB_NUVIOT_ERRS_ACCESS_KEY`; real sender/listener with up-to-30-second wait.
- MqttSenderTests.TestSend: `DEV_MQTT_HOST`, `DEV_MQTT_PORT`, `DEV_MQTT_USERNAME`, `DEV_MQTT_PASSWORD`; real broker.
- RestSenderTests.SendRestMessage: external https://www.nuviot.com.
- CoTSenderTests.SendAsync: external TAK connection using test kevin.zip and slroot.crt fixture data.
The integration tests were excluded, not executed; their success/failure is **not verified**, and no external secrets or persistent services were introduced. Initial unfiltered canonical test attempt timed out after 60 seconds with partial 39 pass/17 fail; the filtered run completed without timeout.

## Discovery caveat
Source inventory found 83 [TestMethod] entries; filtered runner executed 77 (83 minus six tagged integration cases). The emitted stack traces use NUnit Adapter 5.1.0, even though source uses Microsoft.VisualStudio.TestTools.UnitTesting attributes; confirm adapter provenance before attempting broad migration. Two test projects compiled. No test runner discovery errors occurred during the filtered run.
