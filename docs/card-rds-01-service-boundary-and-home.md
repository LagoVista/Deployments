# Card RDS-01 - Define Runtime Data Service Boundary and Home

## Objective

Choose the project/repository home and dependency boundary for the new Runtime Data Service before implementation begins.

## Context

The existing Deployments/AppServices path already authenticates remote runtimes and issues time-limited infrastructure leases. That control-plane behavior should remain. The new service is the data plane: a small, focused, independently scalable host that receives runtime data operations and performs them using cluster-local services and credentials.

## Decisions to make

- Confirm whether the host belongs in AppServices, a new project within an existing repo, or a dedicated lightweight repo.
- Define the maximum dependency set for the host.
- Identify which CloudStorage projects can be referenced directly without dragging in unnecessary web/application layers.
- Identify shared downstream model assemblies required for devices, telemetry, usage, transactions, notifications, and related payloads.
- Decide naming for the service, project, image, Kubernetes deployment/service, and external route.

## Recommended dependency rule

The runtime data host should ideally depend on:

1. Core/common infrastructure.
2. Shared domain model assemblies.
3. CloudStorage repository/provider assemblies required for persistence.
4. Small internal service abstractions only where they avoid duplication without importing a large composition root.

It should not require the full Deployments application or portal composition tree just to write runtime data.

## Deliverables

- Short architecture note naming the chosen home.
- Initial project/solution skeleton location.
- Explicit allowed dependency list.
- Explicit forbidden/unwanted dependency list.
- Initial service name and route convention.
- Cross-reference from the project plan if the service lands outside Deployments.

## Acceptance criteria

- [ ] Service home is decided and documented.
- [ ] The host can be built/deployed independently of the main AppServices process.
- [ ] Dependency budget is documented before endpoint implementation begins.
- [ ] CloudStorage reuse strategy is documented.
- [ ] Shared model dependencies are identified.
- [ ] No runtime-facing contract exposes a database, table, collection, exchange, queue, or connection-string concept unless that concept is genuinely part of the runtime domain.