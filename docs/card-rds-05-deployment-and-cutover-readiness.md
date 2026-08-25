# Card RDS-05 - Deploy, Observe, and Prepare Server Cutover

## Objective

Deploy the Runtime Data Service into the cluster and prove it can safely carry production-style runtime traffic before Phase 2 disables direct infrastructure access.

## Cluster deployment

- Kubernetes Deployment and Service.
- External HTTPS route/ingress suitable for remote runtimes.
- Multiple replicas supported from the first deployment.
- Resource requests/limits with conservative initial values.
- Pod disruption/readiness behavior appropriate for stateless traffic.
- Cluster-local backend connectivity only.
- Secrets/configuration sourced through the standard cluster mechanisms.

## Observability

Capture at least:

- Requests by operation and result.
- Latency percentiles by operation.
- Batch sizes and records processed.
- Authentication/authorization failures.
- Backend failures/retries.
- Active replica count/readiness.
- Idempotency/replay outcomes where applicable.

Logs should carry request/correlation ID, org ID, instance ID, host ID when available, capability, and operation without leaking tokens or infrastructure credentials.

## Validation

Exercise the service with representative runtime traffic and failure cases:

- Valid/expired/invalid leases.
- Lease renewal while traffic continues.
- Multiple service replicas.
- Pod restart during runtime traffic.
- Backend transient failure/retry.
- Duplicate/replayed writes.
- Batch payload limits.
- Device read/write parity.
- Transaction parity.
- Notification delivery.

## Cutover controls

- Keep old infrastructure lease endpoints available during initial rollout.
- Provide a server-side or runtime-version-based way to enable the new Runtime Data Service path selectively.
- Define rollback to the old runtime image/lease path until Phase 2 is complete.
- Do not revoke old infrastructure lease issuance until all supported runtime containers have migrated.

## Deliverables

- Git-managed Kubernetes manifests/Helm values in the appropriate infrastructure repo.
- Validation/runbook.
- Dashboards/alerts or integration with existing platform diagnostics.
- Cutover checklist referencing engine Cards RDS-06 through RDS-10.
- Explicit criteria for disabling each old infrastructure lease endpoint.

## Acceptance criteria

- [ ] Runtime Data Service is deployed in dev with 2+ replicas.
- [ ] External HTTPS access works with a valid Runtime Access Lease.
- [ ] Cluster credentials are not present in runtime responses or logs.
- [ ] Restarting a service pod does not interrupt sustained test traffic beyond normal request retry behavior.
- [ ] Metrics/logging make operation failures attributable to an org/instance/request.
- [ ] All Phase 1 operation families required by the engine migration are available.
- [ ] A written cutover and rollback procedure exists before Phase 2 removes old runtime implementations.