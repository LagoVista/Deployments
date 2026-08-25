# Card RDS-03 - Usage Activity Batch Path and Cassandra Migration

## Objective

Move runtime usage writes behind signed RuntimeData HTTP and migrate the largest append-only usage table from Azure Table Storage to Cassandra activity storage.

## Workload

- Runtime emits once per minute.
- Each instance produces one top-level aggregate plus roughly 20-50 component records.
- Normal web view requests exactly the latest 30 top-level instance snapshots.
- Module/component history is queried only on drill-down.
- Runtime remains responsible for composing all aggregates.

## Target storage shape

Use `IActivityRecordStore<UsageMetrics>` / Cassandra.

- partition: `OrganizationId + InstanceId + Day bucket`
- clustering: `CreationDate + Id`
- optional indexed component/scope field(s) only where drill-down callers require them
- bounded TTL/retention policy, chosen explicitly

The Activity Record provider already supports `InsertBatchAsync`; RuntimeData should forward one natural one-minute cohort as a batch. Keep the HTTP safety ceiling at 250 records.

## Repository conversion

`UsageMetricsRepo` becomes an ordinary class composed with `IActivityRecordStore<UsageMetrics>`; it must no longer inherit `TableStorageBase<UsageMetrics>`.

Convert legacy Azure keys/timestamps into normal activity identity and `CreationDate`. Add a definition-driven historical migration entry in `nuviot/appsupport/LagoVista.StorageMigration`.

## Acceptance criteria

- [ ] `UsageMetrics` implements the activity-record contract and no longer carries Azure-only persistence mechanics.
- [ ] Cassandra definition is colocated with the repo.
- [ ] Day bucketing is supported by the semantic storage layer.
- [ ] Batch write maps directly to `InsertBatchAsync`.
- [ ] Latest-30 top-level instance query is covered.
- [ ] Module drill-down query is covered.
- [ ] TTL is explicitly selected and validated.
- [ ] Historical Azure migration definition/checkpointing is added.
- [ ] Runtime no longer receives Usage Table Storage credentials.

## Status

**In progress.** Signed batch endpoint and repo batch seam exist; Cassandra conversion is next.