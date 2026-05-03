# QAMS

QAMS is a life sciences and medtech-first Quality Assurance Management System designed as an Azure-native SaaS platform.

## Current Implementation Baseline

This repository now contains the first implementation scaffold:

- Backend solution with .NET minimal API services.
- Shared backend building blocks for API responses, headers, audit, events, e-signatures, and workflows.
- Frontend shell scaffold for a future React micro-frontend workspace.
- Full backend service boundary scaffold for platform, domain, and intelligence services.
- Baseline backend test project for shared audit hashing.
- Azure Bicep infrastructure skeleton.
- Helm deployment skeleton.
- Production design and implementation roadmap documents.

The production target in the design documents is .NET 10 LTS. This workstation currently has .NET 9 installed, so the starter projects target `net9.0` to keep the scaffold buildable locally. Upgrade the target framework and SDK pin once .NET 10 is installed in the development environment.

## Repository Layout

```text
src/
  backend/
    Qams.sln
    services/
    shared/
  frontend/
    apps/
    packages/
infra/
  bicep/
deploy/
  helm/
docs/
tests/
```

## Backend Quick Start

```powershell
dotnet restore src/backend/Qams.sln
dotnet build src/backend/Qams.sln
dotnet test src/backend/Qams.sln -m:1
dotnet run --project src/backend/services/Qams.Capa.Api/Qams.Capa.Api.csproj
```

## Frontend Quick Start

The frontend scaffold is intentionally dependency-light and ready for a React/Vite install when package execution is enabled.

```powershell
cd src/frontend/apps/qams-shell
npm install
npm run dev
```

## First Vertical Slice

The first implementation slice is the regulated SaaS platform foundation:

1. Tenant provisioning.
2. Workflow definition.
3. Quality event creation.
4. CAPA creation and guarded closure with e-signatures.
5. Audit ledger verification with hash chain.
6. E-signature service for regulated actions.
7. Policy evaluation for RBAC/ABAC.
