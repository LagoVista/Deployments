# Runtime Data Service Project Plan

## Goal

Replace direct infrastructure access from remote IoT runtimes with a small, stateless, horizontally scalable cluster-facing Runtime Data Service. Remote runtimes continue to obtain short-lived access through the existing Deployments/AppServices control plane, but leased credentials authorize calls to NuvIoT-owned runtime APIs rather than exposing Mongo, Cassandra, PostgreSQL, RabbitMQ, Azure Table Storage, Cosmos DB, or other infrastructure credentials.

## Architectural boundaries

- `InstanceRuntimeController` remains the control-plane entry point for authenticated runtime requests and lease issuance.
- The new Runtime Data Service is a separate data-plane service optimized for runtime traffic.
- Cluster infrastructure credentials never leave the cluster.
- The Runtime Data Service should be stateless and horizontally scalable.
- Prefer shared domain models from existing downstream projects.
- Prefer existing CloudStorage repositories/managers where they provide the required behavior without pulling in large application-service dependency graphs.
- Do not require the Runtime Data Service to share all existing service abstractions. Share interfaces only when they keep the dependency graph small and preserve useful behavior.
- Runtime-facing contracts should express business/runtime operations, not persistence-provider concepts.
- Batch endpoints should be supported where runtime traffic is naturally append-heavy.

## Service home

The final repository/project home is intentionally not fixed yet. `AppServices` is the leading candidate because lease issuance already belongs to the main application-services pipeline, but the data-plane host must remain independently deployable and have a deliberately small dependency budget. Card 1 resolves this before implementation spreads across repositories.

## Phase 1 - Server side

1. [Card 1 - Runtime Data Service boundary and home](card-rds-01-service-boundary-and-home.md)
2. [Card 2 - Runtime access lease contract](card-rds-02-runtime-access-lease.md)
3. [Card 3 - Runtime Data Service host and authentication](card-rds-03-runtime-data-service-host.md)
4. [Card 4 - Runtime data operation contracts and adapters](card-rds-04-runtime-data-operations.md)
5. [Card 5 - Cluster deployment, observability, and cutover readiness](card-rds-05-deployment-and-cutover-readiness.md)

## Phase 2 - Runtime

Runtime implementation work is tracked in `nuviot/engine` as Cards 6-10. The containerized runtimes have no compatibility requirement with future unreleased runtime versions, so Phase 2 should refactor aggressively toward the end-state API client and remove direct infrastructure SDKs where practical.

## Migration principles

- Build and validate the new path before disabling old leases.
- Allow old and new paths to coexist during migration.
- Move simple append-only writers first.
- Move Rabbit/PostgreSQL direct connections next.
- Move richer device-storage/query behavior after the transport and authorization path is proven.
- Disable and ultimately delete direct infrastructure lease issuance only after the corresponding runtime path is verified.

## Initial runtime infrastructure inventory

The current engine contains direct consumers for at least:

- Cosmos DB device storage.
- Azure Table Storage device connection events.
- Azure Table Storage usage metrics.
- Azure Table Storage device status/archive/exceptions/sensor archives and related remote storage.
- PostgreSQL device transactions.
- RabbitMQ notification publishing.
- Additional logging/checkpoint/PEM paths that require final active-use classification.

## Completion definition

The project is complete when supported remote runtimes communicate only with NuvIoT-owned HTTPS endpoints for these platform data operations, renew a short-lived runtime access lease without learning cluster credentials, and no supported runtime requires direct network access to cluster persistence or messaging systems.