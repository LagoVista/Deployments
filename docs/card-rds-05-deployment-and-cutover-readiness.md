# Card RDS-05 - API Wiring, Observability, and Cutover

## Objective

Finish the first RuntimeData vertical slices inside the existing API host, measure them, and retire the corresponding infrastructure credential paths only after validation.

## API integration

- Reference `LagoVista.IoT.RuntimeData` from the existing `nuviot/nuviot` API composition.
- Add the RuntimeData controller assembly to `AddLagoVistaControllers()`.
- Reuse existing DeviceManagement, Deployment, CloudStorage, SecureStorage, and WebCommon registrations.
- Do not introduce a new executable host for V1.

## Observability

Capture by RuntimeData operation:

- request rate
- duration/latency
- success/failure
- batch size / records accepted
- payload size where useful
- signed-request validation failures
- backend/storage failures
- org/instance dimensions only where cardinality is acceptable

Use these measurements to decide later whether RuntimeData needs an independent host. Do not pre-scale architecture without evidence.

## First cutover set

1. Usage metrics: signed batch HTTP -> Cassandra activity store.
2. Device connection/history: signed HTTP -> Cassandra activity store.
3. Current device connectivity projection: signed HTTP -> durable mutable internal store.

After each path is validated, remove/disable the corresponding runtime credential/settings endpoint and direct provider SDK use in `nuviot/engine`.

## Validation

- signed valid/invalid runtime calls
- usage one-minute batch behavior
- latest-30 instance usage read
- module drill-down
- connection history write/read
- current device state update/read/list/timed-out queries
- storage TTL where configured
- API restart/pod restart with current state preserved
- no infrastructure credentials in runtime responses/logs

## Acceptance criteria

- [ ] RuntimeData controllers are hosted by the existing API server.
- [ ] Endpoint-family metrics are available.
- [ ] Usage and connection-history semantic stores are validated.
- [ ] Current connectivity projection is durable and query-complete.
- [ ] Engine uses signed API paths for the migrated operations.
- [ ] Old Usage/Connection/Status storage credential endpoints are removed only after parity validation.
- [ ] Separate host remains an evidence-driven future option, not a prerequisite.

## Status

**Pending completion of Cards 2-4.**