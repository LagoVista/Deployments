# Card RDS-03 - Build Runtime Data Service Host

## Objective

Create the minimal stateless HTTP service that accepts runtime data-plane requests inside the cluster boundary.

## Responsibilities

- Validate Runtime Access Leases from Card RDS-02.
- Establish runtime request context: org, instance, host, capability.
- Expose versioned runtime-facing HTTPS APIs.
- Provide consistent validation, error responses, correlation IDs, metrics, and structured logging.
- Resolve and call cluster-local repositories/services using cluster-held credentials.
- Remain stateless so Kubernetes can scale replicas horizontally.

## Non-goals

- Do not move lease issuance into this service.
- Do not expose generic database proxy APIs.
- Do not reproduce Cosmos/Table/Rabbit/Postgres protocols.
- Do not make the runtime aware of Kubernetes service names or storage-provider topology.

## Baseline service capabilities

- Health/readiness endpoints.
- Runtime lease authentication middleware/filter.
- Capability authorization.
- Request-size and batch-size limits.
- Idempotency/correlation mechanism for write operations where replay is possible.
- OpenAPI/contract documentation suitable for generating or sharing client contracts if useful.

## Performance posture

The service should be designed for inexpensive horizontal scale:

- Avoid session state.
- Reuse backend client pools/connections.
- Support batched write APIs.
- Avoid control-plane calls on the normal data request path.
- Capture request duration/error-rate metrics from the start.

## Deliverables

- Buildable host project.
- Authentication/authorization pipeline.
- Health/readiness endpoints.
- Baseline runtime API route/version convention.
- Integration-test harness that can issue a valid test lease and call a protected endpoint.
- Container image definition.

## Acceptance criteria

- [ ] Service builds independently.
- [ ] Service starts with only its documented dependencies/configuration.
- [ ] Unauthenticated and expired-lease requests are rejected.
- [ ] Capability checks are enforced.
- [ ] Valid runtime requests establish org/instance context.
- [ ] Health/readiness checks are available.
- [ ] At least one protected test endpoint proves end-to-end lease validation.