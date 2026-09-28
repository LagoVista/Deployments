# Card RDS-02 - Shared Signed Runtime Request Context

## Objective

Reuse the runtime signed-request authentication already proven by `InstanceRuntimeController` without duplicating cryptographic or identity logic across RuntimeData controllers.

## Decision

The existing runtime shared keys and `ISignedRequestHttpValidator.ValidateRuntimeInstanceHttpV1(...)` remain authoritative. RuntimeData does not issue or validate a second token/lease.

Each RuntimeData request establishes:

- Organization
- User
- Deployment instance
- Host when needed
- signed request validity

Payload identity is then bound to that context.

## Initial implementation

The first RuntimeData controller already uses the existing deployment instance repository, secure storage, and `ISignedRequestHttpValidator`. Any reusable extraction should reduce controller boilerplate while preserving the exact existing validation semantics.

## Acceptance criteria

- [x] No RuntimeData access lease exists.
- [x] Existing runtime shared keys/signatures are reused.
- [x] RuntimeData rejects payload instance identity that conflicts with the signed runtime.
- [ ] Extract reusable request-context helper/filter if additional controllers would otherwise duplicate material logic.
- [ ] Add focused invalid-signature/wrong-org/wrong-instance coverage.

## Status

**In progress; core authentication path proven by first RuntimeData slice.**