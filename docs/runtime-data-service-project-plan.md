# Runtime Data Service Project Plan

## Goal

Remove direct infrastructure access from remote IoT runtimes. Runtimes call NuvIoT-owned HTTPS endpoints using the existing signed runtime request mechanism; cluster storage and messaging credentials never leave the cluster.

## Final architecture

```text
runtime
  -> RuntimeSignedHttpClient
  -> existing Web API /api/runtime-data/...
  -> existing runtime signature validation
  -> thin RuntimeData controller/service
  -> existing domain repositories
  -> semantic storage/provider
```

There is no Runtime Data Service lease, no new runtime-to-cloud RPC transport, and no separate RuntimeData host in the first version. `LagoVista.IoT.RuntimeData` lives in `nuviot/appsupport` and its controllers are hosted by the existing API server in `nuviot/nuviot`.

## Restacked first five cards

1. [RDS-01 - Boundary, home, and existing API integration](card-rds-01-service-boundary-and-home.md)
2. [RDS-02 - Signed runtime request context](card-rds-02-runtime-access-lease.md)
3. [RDS-03 - Usage activity batch path and Cassandra migration](card-rds-03-runtime-data-service-host.md)
4. [RDS-04 - Device connection history and current-state split](card-rds-04-runtime-data-operations.md)
5. [RDS-05 - API wiring, observability, cutover, and credential removal](card-rds-05-deployment-and-cutover-readiness.md)

The original lease/separate-host design is superseded by these cards.

## Storage decisions established during the restack

### UsageMetrics

`UsageMetrics` is treated as high-volume append-only activity storage in Cassandra rather than PostgreSQL metrics storage. Runtime composition remains authoritative. The normal UI query is the latest 30 one-minute top-level instance snapshots, with optional module drill-down.

Target shape:

- partition: OrganizationId + InstanceId + Day bucket
- clustering: CreationDate + Id
- natural runtime batch: one minute of instance + component records
- API safety ceiling: 250 records/request
- TTL: explicit retention policy, to be selected deliberately

### Device connection history

Device connection events and immutable status history belong in Cassandra activity storage.

### Current device connectivity

Current device state is mutable and operationally important. It must not be modeled as append-only history. The target is durable Application Data/Mongo unless caller analysis reveals a stronger consistency requirement. Current state and history should have separate repository responsibilities even if legacy interfaces currently combine them.

## Migration principles

- Runtime owns business/domain composition.
- RuntimeData validates identity, transport shape, batching, and storage-boundary rules only.
- Reuse existing DeviceManagement/DeploymentManagement repos and DI before adding abstractions.
- Move provider-specific Table/Cosmos/Rabbit/Postgres access behind signed endpoints.
- Use semantic storage capabilities rather than replacing one universal repository base with another.
- Batch append-heavy writes naturally.
- Preserve existing infrastructure credential endpoints only until each corresponding runtime consumer has migrated.
- Add endpoint-family metrics from the start; extract a separate host later only if observed load requires it.

## Completion definition

The project is complete when supported remote runtimes use signed NuvIoT HTTPS endpoints for platform data operations, no runtime receives cluster infrastructure credentials, migrated append/history data uses semantic internal storage, and obsolete credential-leasing paths can be removed.