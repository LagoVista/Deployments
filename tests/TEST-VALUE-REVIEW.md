# Task 4 — Failing test value and removal review

Review source: LagoVista/Deployments, Task 3 baseline commit cb498ec1c04df47f261d9e451028916bb2ae617e. Baseline: 77 unit tests, 52 passed, 25 failed. Evidence: softwarelogistics/quality reports branch, reports/lagovista/deployments/cb498ec1c04df47f261d9e451028916bb2ae617e/6661122fa8bba19bc90000bd8e6925384d23731f6de8d3e8751cb3e74fdf26ef/test-results/tests.json.

## Decision: no failing tests qualify for deletion

The 25 failing tests were reviewed individually against their intended behavior, summarized by group below. They fail due to fixture identifier format changes or null references in test setup / code paths. These are not proof that the tests are brittle or have negligible value. Deleting any to turn the suite green would conceal useful behavioral coverage.

### HostDeletionTests — retain all 4
- HostDelete_DeleteNotificationShouldReturnErrHST1001: checks the exact MCP-host deletion error code. Retain.
- HostDelete_DeleteShoudlFailForNotificationserver: prevents deletion of a notification server host. Retain.
- HostDelete_DeleteShouldFailForMCPServer: prevents deletion of an MCP host. Retain.
- HostDelete_DeleteShouldFailForMCPServerErrHST1002: checks exact notification-host deletion error code. Retain.
All fail in setup because MYHOSTID violates NormalizedId32; valid fixture replacement should precede any assertion changes. Note two test names mention the opposite host/error role from their body: later clarify naming only after validating actual expected behavior.

### HostStatusTests — retain all 3
- Host_ShouldUpdate_State_IfStatusIfChanged: status transition behavior. Retain.
- Host_ShouldUpdate_Status_DateIfChangeed: change timestamp semantics. Retain.
- Host_Should_Not_UpdateStatus_IfStatusDidNotChange: protects against redundant updates. Retain.
All fail in setup on MYHOSTID.

### InstanceTests — retain all 11
- Instance_Add_Can_Add_If_Device_Repo_Not_InUse: successful assignment. Retain.
- Instance_Add_Does_Not_Call_Add_If_Repo_Is_InUse: prevents writes when repo is occupied. Retain.
- Instance_Add_ThrowValidationException_Device_Repo_Is_InUse: validates conflict rejection. Retain.
- Instance_Add_ThrowValidationException_IfDeviceRepoNotAssigned: validates required association. Retain.
- Instance_Delete_ClearRepo: releases repository association. Retain.
- Instance_Delete_DeletesHostIfDedicated: dedicated-host cleanup. Retain.
- Instance_Delete_ShouldNotDeleteIfNotDedicated: shared-host preservation. Retain.
- Instance_Update_DeviceRepoChange_NewDeviceRepoInUse_WillNotCallUpdatesn: prevents writes on a conflicting reassignment. Retain.
- Instance_Update_DeviceRepoChange_NewDeviceRepoInUse_WillThrowException: rejects conflicting reassignment. Retain.
- Instance_Update_DeviceRepoChange_NewDeviceRepoNotInUse: allowed reassignment. Retain.
- Instance_Update_NoDeviceRepoChange: avoids needless updates. Retain.
Failures are invalid short values (INSTANCEID001, MYHOSTID, ABC123ABC123ABC123, etc.). Preserve distinct identity and negative-case expectations when repairing fixtures.

### InputTranslatorToInputValidationTests — retain all 4
- InputTranslator_To_Input_StateSetMisMatch_InValid: invalid state-set mapping detection. Retain.
- InputTranslator_To_Input_StateSet_Valid: valid state-set mapping. Retain.
- InputTranslator_To_Input_UnitSetMisMatch_InValid: invalid unit-set mapping detection. Retain.
- InputTranslator_To_Input_UnitSet_Valid: valid unit-set mapping. Retain.
Their stateset1/unitset1 identifiers violate NormalizedId32. Avoid concatenating arbitrary suffixes to valid IDs in mismatch tests because that can produce invalid IDs; use two distinct valid IDs and preserve validation semantics.

### Notifications — retain all 3
- EmailSenderTests.SendTest: mocked-email delivery path. Retain; NullReferenceException at EmaiLSender.cs:111.
- NotificationSenderTests.SendNotificationAsync: raised-notification orchestration behavior. Retain; NullReferenceException at NotificationSender.cs:224.
- SMSSenderTests.SendTest: mocked-SMS delivery path. Retain; NullReferenceException at SMSSender.cs:101.
These cover useful interactions; investigate null fixture data/mock returns before changing production behavior.

## Passing but weak tests — NOT removal candidates under Task 4
- CoTSenderTests.LoadCerts has an empty test body and no assertions. It passed, so removing it would not repair a failing test. Future cleanup may replace it with actual certificate validation proof.
- NotificationSenderTests.UniqueNotificationsTest prints contacts but has no assertions. It passed; could strengthen with explicit deduplication assertions in a follow-on effort.
Neither is deleted here.

## Outcome and scope
Removed failing tests: **0**. Removed coverage: **0**. All 25 remain eligible for repair in Task 5. This is a conscious evidence-based decision, not a skipped cleanup step. No changes to production implementation, test timeouts or global test policy are authorized by this review.
