# Card RDS-01 - Runtime Data Boundary and Existing API Home

## Objective

Establish the runtime-to-cloud boundary using the infrastructure that already exists.

## Decision

- `LagoVista.IoT.RuntimeData` lives in `nuviot/appsupport` as a library/package.
- Controllers are hosted by the existing API server in `nuviot/nuviot`.
- Remote runtimes call `/api/runtime-data/...` using `RuntimeSignedHttpClient`.
- No separate RuntimeData executable/host is required for V1.
- No Runtime Data lease is introduced.
- Runtime-to-cloud traffic does not use Core RPC or Service Bus.

## Dependency rule

RuntimeData should be thin. Prefer existing DeploymentManagement/DeviceManagement repo/manager interfaces already registered by their normal modules. Add a new RuntimeData service/repository layer only where an actual operation requires behavior not already represented.

## Security boundary

The signed runtime instance identity is authoritative. Payload org/instance identity must be derived from or validated against the signed request context; arbitrary payload identity is never trusted.

## Acceptance criteria

- [x] Package home selected: `nuviot/appsupport`.
- [x] Host selected: existing `nuviot/nuviot` API server.
- [x] Signed HTTP selected for runtime -> cloud.
- [x] Separate lease/host/RPC designs explicitly rejected for V1.
- [x] Existing Deployment/DeviceManagement DI and repositories are the default implementation path.

## Status

**Complete.**