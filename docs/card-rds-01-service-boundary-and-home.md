# Card RDS-01 - Define Runtime Data Service Boundary and Home

## Objective

Choose the project/repository home and dependency boundary for the new Runtime Data Service before implementation begins.

## Decision

The Runtime Data Service will live in **`nuviot/nuviot`** as an independently deployable ASP.NET Core host project:

- Project: `src/LagoVista.RuntimeData.Host`
- Assembly/project name: `LagoVista.RuntimeData.Host`
- Service name: `runtime-data-service`
- Container image: `nuviot/runtime-data-service`
- Kubernetes Deployment/Service: `runtime-data-service`
- External API prefix: `/api/runtime-data/v1`
- Health/readiness: `/health` and `/ready`

The existing Deployments/AppServices path remains the **control plane** for runtime authentication and lease issuance. The Runtime Data Service is a separate **data plane** and will be built, deployed, and scaled independently.

The initial host skeleton is intentionally a plain `Microsoft.NET.Sdk.Web` project with no LagoVista package dependencies. Dependencies will be added only as concrete RDS cards require them.

## Why `nuviot/nuviot`

`nuviot/nuviot` already contains the actively deployed application hosts and host-composition patterns. A sibling host keeps service build/container conventions together while avoiding a new repository for a deliberately small service.

The existing `LagoVista.IoT.WebHost.Common` project is **not** a default dependency for this service. It currently brings a broad web/application composition graph including UserAdmin, SecureStorage, Rabbit notification publishing, RuntimeTokenManager, multiple user-facing authentication providers, background processing, and other functionality that the runtime data plane does not need.

## Dependency budget

### Preferred/allowed

Add only when required by a concrete runtime operation:

1. ASP.NET Core hosting/runtime packages.
2. `LagoVista.Core` and narrowly-scoped common infrastructure.
3. Configuration/bootstrap support needed to resolve cluster-local settings.
4. Shared domain model assemblies rather than duplicate DTOs where those models are safe runtime contracts.
5. Focused CloudStorage/repository packages that implement the required persistence operation.
6. Small service abstractions where they reduce duplication without importing an application composition root.

### Initial shared model sources

The initial runtime operation inventory points to shared models from:

- `LagoVista/DeviceManagement` for `Device`, `DeviceConnectionEvent`, `DeviceTransaction`, device summaries/status data, and related device-domain payloads.
- `LagoVista/Deployments` for runtime lease/control-plane models and usage/runtime settings models where still applicable.
- `LagoVista/Runtime` / the corresponding runtime model packages for notification/message contracts that are genuinely shared across server and runtime.
- `LagoVista/Core` for common entity headers, validation/results, IDs, and cross-cutting primitives.

RDS-04 must prefer these model packages while allowing a runtime-specific request/response DTO when versioning, batching, security, or transport semantics justify one.

### CloudStorage reuse rule

Prefer provider-neutral repositories and focused provider implementations from `LagoVista/CloudStorage` and downstream `*.Repos` packages. Do **not** import a manager/application layer merely to gain access to a repository if a small local adapter can compose the repository directly.

### Explicitly avoid by default

- `LagoVista.IoT.WebHost.Common` as a blanket dependency.
- Portal/UI projects.
- Full application-service composition roots.
- User-facing OAuth/social authentication packages.
- Unrelated managers/repositories copied from the main Web API dependency graph.
- Provider SDKs until an implemented data operation actually requires that provider inside the cluster.
- Any runtime-facing database, table, collection, queue, exchange, connection-string, or credential concept.

## Responsibility split

### Deployments / AppServices control plane

- Validate the existing signed runtime identity.
- Issue and renew short-lived Runtime Access Leases.
- Bind leases to org/instance/host and capabilities.
- Never carry normal high-volume runtime data traffic.

### Runtime Data Service data plane

- Validate Runtime Access Leases locally.
- Authorize capabilities.
- Accept runtime-domain operations over HTTPS.
- Use cluster-local repositories/services and credentials.
- Remain stateless and horizontally scalable.

## Skeleton

The host skeleton is tracked in `nuviot/nuviot` on `feature/runtime-data-service` under `src/LagoVista.RuntimeData.Host`.

The skeleton currently provides only startup plus `/health`, `/ready`, and root service metadata. Lease authentication and protected endpoints belong to RDS-02/RDS-03.

## Acceptance criteria

- [x] Service home is decided and documented.
- [x] The host is structurally independent of the main AppServices process.
- [x] Dependency budget is documented before endpoint implementation begins.
- [x] CloudStorage reuse strategy is documented.
- [x] Shared model dependency sources are identified.
- [x] No runtime-facing contract exposes a database, table, collection, exchange, queue, or connection-string concept.

## Status

**Complete.** Continue with RDS-02 - Runtime Access Lease.
