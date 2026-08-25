# Card RDS-04 - Implement Runtime Data Operations and Storage Adapters

## Objective

Implement the runtime-facing operations required to replace direct infrastructure access, reusing shared models and CloudStorage/provider logic where practical.

## Initial operation families

### Append/write-first operations

- Device connection events.
- Usage metrics.
- Device status/archive/exception/sensor archive writes confirmed as active.
- Runtime logging paths confirmed as active.

These are the preferred first production slices because they are naturally simple, append-oriented operations and are good validation of transport, lease authorization, batching, retries, and observability.

### Notification publishing

Accept the existing shared notification model and publish internally through the cluster's notification/Rabbit abstraction. The remote runtime must not receive Rabbit host/user/password/topology details.

### Device transactions

Move transaction persistence semantics server-side, including balance lookup/update, hashing/integrity behavior, and database interaction. The runtime submits the domain transaction request and receives the resulting balance/outcome.

### Device repository operations

Implement the concrete operations currently required by `IDeviceStorage`, including device lookup, update/upsert, configuration-based queries, device-group queries, and any additional active methods discovered during runtime inventory. The API should model these as device operations, not Cosmos queries.

### Remaining active infrastructure paths

Classify and implement replacement APIs for Event Hub checkpoint, PEM, media, logging, and other leased resources only where the current supported runtime actually uses them.

## Adapter guidance

- Prefer existing CloudStorage repositories/provider-neutral abstractions when they already express the required operation.
- It is acceptable to create thin service-local adapters when existing manager/service abstractions introduce unnecessary dependencies.
- Reuse domain models rather than duplicating DTOs unless a runtime-specific request/response contract materially improves versioning or security.
- Keep persistence decisions server-side.
- Add batch forms for high-volume append operations.

## Compatibility and idempotency

- Define retry semantics per operation.
- Writes that may be replayed need idempotency keys or naturally idempotent identities.
- Preserve relevant runtime-visible behavior from the old direct implementations.
- Do not preserve backend implementation quirks merely for compatibility.

## Deliverables

- Endpoint/operation matrix mapping old runtime interface methods to new APIs.
- Server implementations for each required operation family.
- CloudStorage/service adapters.
- Batch contracts for append-heavy paths.
- Integration tests against the actual in-cluster/provider-neutral repository implementations.
- Parity tests for device and transaction behavior where semantics are richer than simple writes.

## Acceptance criteria

- [ ] Every direct infrastructure consumer targeted for Phase 2 has a corresponding runtime API operation or an explicit decision that it is obsolete.
- [ ] Simple write paths support retry-safe behavior.
- [ ] High-volume append paths support batching.
- [ ] Notifications are published without exposing Rabbit credentials.
- [ ] Transactions execute fully server-side.
- [ ] Device storage operations no longer require Cosmos semantics in the runtime-facing contract.
- [ ] Provider-specific credentials remain entirely inside the cluster.