# Card RDS-02 - Introduce Runtime Access Lease

## Objective

Replace infrastructure-specific leased credentials with a short-lived lease that authorizes a remote runtime to call NuvIoT-owned runtime data APIs.

## Scope

Keep the current runtime authentication path and `InstanceRuntimeController` control-plane responsibility. Introduce a new lease contract that can be obtained and renewed through the existing main application-services pipeline.

## Lease contents

The lease should bind at least:

- Organization ID.
- Instance ID.
- Host ID when applicable.
- Issued/valid-through timestamps.
- Runtime data endpoint or service discovery information.
- Allowed capabilities/scopes.
- Token/version metadata required for validation and rotation.

Possible capabilities include:

- `device.read`
- `device.write`
- `device-events.write`
- `usage.write`
- `transactions.write`
- `notifications.publish`
- additional capabilities identified during endpoint inventory.

## Design notes

- Prefer one renewable Runtime Data Service lease containing multiple capabilities over one infrastructure lease per backend.
- Preserve the runtime's existing expiration/renewal behavior where practical.
- The lease must not contain Mongo, Cassandra, PostgreSQL, RabbitMQ, Cosmos DB, Azure Storage, or other infrastructure credentials.
- Validation should be local and inexpensive for the Runtime Data Service. Prefer signed tokens/claims over a control-plane round trip for every data request.
- Leave existing lease endpoints functional during migration.

## Deliverables

- Shared runtime lease model.
- Lease issuer service integrated with the current runtime authentication path.
- New runtime lease endpoint or backwards-compatible evolution of an existing endpoint.
- Validation/signing key strategy.
- Capability naming/versioning rules.
- Unit/integration tests covering expiration, invalid signature, wrong instance/org, missing capability, and successful renewal.

## Acceptance criteria

- [ ] Authenticated runtimes can obtain a short-lived Runtime Data Service lease.
- [ ] Lease identifies org/instance and allowed capabilities.
- [ ] Runtime Data Service can validate the lease without infrastructure credentials leaving the cluster.
- [ ] Lease expiry and renewal behavior are test-covered.
- [ ] Existing infrastructure lease issuance remains available until cutover cards explicitly disable it.