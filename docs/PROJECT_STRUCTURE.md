# QAMS Project Structure

## Root

- `QAMS_Final_Design_Document.md`: production product and architecture design.
- `QAMS_Implementation_Roadmap.md`: delivery roadmap and release gates.
- `QAMS_Architecture_Diagram.md`: source-controlled Mermaid architecture diagrams.
- `src/backend`: .NET backend solution, services, and shared libraries.
- `src/frontend`: frontend applications and shared UI packages.
- `infra/bicep`: Azure infrastructure as code.
- `deploy/helm`: Kubernetes deployment charts.
- `tests`: integration, contract, UI, performance, security, compliance, and AI evaluation tests.

## Backend

```text
src/backend/
  Qams.sln
  services/
    Qams.Identity.Api/
    Qams.AuditLedger.Api/
    Qams.Workflow.Api/
    Qams.QualityEvents.Api/
    Qams.Capa.Api/
  shared/
    Qams.BuildingBlocks/
```

## Frontend

```text
src/frontend/
  apps/
    qams-shell/
  packages/
    design-system/
    api-client/
```

## Future Backend Services

Add new services under `src/backend/services` using the same pattern:

- `Qams.DocumentControl.Api`
- `Qams.Training.Api`
- `Qams.ChangeControl.Api`
- `Qams.AuditInspection.Api`
- `Qams.Complaints.Api`
- `Qams.SupplierQuality.Api`
- `Qams.RiskDesignControls.Api`
- `Qams.ProductMaster.Api`
- `Qams.EquipmentCalibration.Api`
- `Qams.Reporting.Api`
- `Qams.Search.Api`
- `Qams.AiAssistant.Api`
- `Qams.ValidationEvidence.Api`
- `Qams.Integration.Api`

## Rules

- Shared contracts belong in `Qams.BuildingBlocks`.
- Services own their data and do not write into another service database.
- Mutating APIs require `Idempotency-Key`.
- Tenant-owned APIs require `X-Tenant-Id`.
- Regulated actions must create audit ledger entries.
- E-signatures must link to record hashes and audit entries.
