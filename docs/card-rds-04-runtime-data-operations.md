# Card RDS-04 - Device Connectivity History and Current-State Split

## Objective

Move device connectivity data off Azure Table Storage without conflating immutable history with authoritative current state.

## Legacy shape

The existing repositories mix two personalities:

1. immutable history (`DeviceConnectionEvent`, device status history)
2. mutable current status (`GetDeviceStatusAsync`, add/update status, watchdog/timed-out/current-device listing)

They currently use different physical Azure tables but share repository interfaces and DTOs.

## Decision

### Connection and status history

Move append-only device connection/history records to `IActivityRecordStore<T>` / Cassandra.

The history record should use normal activity fields (`Id`, `OrganizationId`, `Organization`, `CreationDate`) and preserve device identity plus relevant connection/status details. Partition and bucket shape must match real per-device history queries; do not recreate global Table Storage scans.

### Current connectivity state

Do **not** model current state as Cassandra activity history. Current status is mutable, important operational state and supports:

- get one device's current status
- update current state/last contact/watchdog information
- list current device status for a repository/instance
- find timed-out devices

Target this state at durable `IApplicationDataStore` / Mongo unless focused caller/concurrency analysis identifies a stronger requirement. Give current state and history separate repository responsibilities even if public manager APIs remain stable.

## RuntimeData

The runtime posts connection/history events through signed HTTP. Current-state updates may be carried in the same runtime call when one event naturally changes both projections, but server storage remains two explicit operations:

```text
connection event -> Cassandra history
current state change -> Application Data current projection
```

Do not reconstruct current connectivity by scanning history on normal request paths.

## Acceptance criteria

- [ ] `DeviceConnectionEvent` history is stored via Cassandra activity storage.
- [ ] Device status history is classified/migrated to Cassandra where still actively used.
- [ ] Current device status is separated from history storage.
- [ ] Current-state query requirements (single device, repository/instance list, timed-out set) are covered by explicit Application Data indexes.
- [ ] RuntimeData write path does not expose Azure credentials.
- [ ] Historical migration definitions are added for the legacy connection/status-history tables.
- [ ] Current state survives process/pod restarts and is not dependent on replaying history.

## Status

**In progress.** Connection-event signed endpoint exists; semantic storage conversion and current-state projection split remain.