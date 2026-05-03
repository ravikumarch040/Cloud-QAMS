# QAMS Final Design Document

Status: Production design baseline
Audience: Product, engineering, quality/compliance, security, operations
Primary market: Life sciences and medtech
Architecture baseline: Azure-native SaaS using .NET microservices, micro-frontends, API Management, AKS, Service Bus, Event Grid, Azure SQL, Cosmos DB, Blob Storage, Redis, Azure AI Search, Azure OpenAI, and Azure observability services.

## 1. Executive Summary

QAMS will be a modern Quality Assurance Management System for regulated life sciences and medtech organizations. The product must compete with established eQMS platforms by delivering full QMS module breadth, configurable workflow automation, audit-ready evidence, validated compliance controls, AI-assisted quality work, supplier/product traceability, and an easy role-based user experience.

The current architecture and blueprint files already establish the right Azure cloud-native direction. The required design upgrade is to make the platform regulated-by-design, workflow-first, validation-ready, AI-governed, and operationally production-grade.

## 2. Market Gap Analysis

Modern eQMS competitors have moved beyond basic document, audit, and CAPA tracking. Leading products emphasize closed-loop quality, configurable workflows, AI assistants, compliance intelligence, supplier collaboration, validation evidence, mobile/offline execution, and continuous audit readiness.

| Market capability | Seen in current leaders | QAMS design response |
| --- | --- | --- |
| AI quality assistants | Veeva AI agents, Dot Compliance Dottie, Qualio compliance intelligence, Ideagen AI | Governed QAMS Copilot with citations, human approval, prompt/model audit logs, and regulated action boundaries |
| Compliance intelligence | Qualio gap analysis, Ideagen regulatory monitoring, ComplianceQuest QMSR toolkits | Regulatory Controls service mapping FDA QMSR, ISO 13485, ISO 9001, 21 CFR Part 11, EU Annex 11, and tenant procedures to live evidence |
| Workflow configurability | AssurX configurable workflows, Intellect no-code positioning, ETQ-style platform apps | Workflow, Form, and Rules services with versioned definitions, controlled changes, and validation impact assessment |
| Closed-loop quality | MasterControl, Veeva, Greenlight Guru, AssurX | Quality Event hub linking deviations, NCs, complaints, audits, CAPA, changes, training, risks, suppliers, products, and documents |
| Validation and audit readiness | MasterControl validation, Dot Compliance validation packages, regulated cloud platforms | Validation Evidence service, release evidence packs, CSA/GAMP-aligned test artifacts, traceable requirements, immutable audit ledger |
| Design/risk/product traceability | Greenlight Guru design controls and trace graphs, ComplianceQuest PLM/QMS link | Risk and Design Controls service linked to Product/Device Master, requirements, tests, complaints, CAPAs, and change controls |
| Supplier quality | Veeva external collaboration, AssurX supplier portal, ComplianceQuest PartnerQuest | Supplier portal, SCAR workflows, supplier risk scoring, qualification, audits, certificates, incoming inspection integration |
| Mobile/offline work | Audit and inspection mobile features across several leaders | Offline-capable PWA/native shell for audits, inspections, shop-floor evidence, photo capture, barcode/QR scan, and sync conflict resolution |

## 3. Product Principles

- Regulated by design: compliance controls, audit trails, e-signatures, retention, validation evidence, and data integrity are platform primitives.
- Automation before administration: routine routing, training assignment, escalation, impact assessment, evidence collection, and report generation should be automated.
- Human accountable AI: AI may draft, summarize, recommend, and detect gaps, but regulated closure, approvals, and signatures remain human-owned.
- Closed-loop quality: every issue can be traced from detection to root cause, action, verification, training, document change, risk update, and management review.
- Configurable without losing control: forms, workflows, rules, fields, and dashboards are tenant-configurable with versioning, approvals, impact assessment, and validation evidence.
- Fast for daily users: operators, auditors, suppliers, and QA teams should complete common work from focused work queues instead of navigating module silos.

## 4. Personas And Experience Model

| Persona | Primary jobs | UX requirements |
| --- | --- | --- |
| QA Manager | Own QMS health, CAPA, audits, compliance, management review | Quality command center, risk heatmaps, overdue work, audit readiness, evidence export |
| Quality Engineer | Investigate events, manage CAPA, root cause, effectiveness | Guided investigation workspace, linked records, RCA tools, AI summaries, task automation |
| Document Owner | Create, revise, review, approve SOPs and policies | Controlled authoring, version compare, approval routing, periodic review, training impact |
| Operator / Frontline User | Follow current procedures, report issues, complete training | Simple task inbox, mobile access, QR/barcode launch, current effective SOPs, minimal fields |
| Auditor / Inspector | Plan and perform audits, collect findings, verify evidence | Mobile/offline checklists, evidence capture, finding-to-CAPA conversion, export packs |
| Supplier / Partner | Respond to SCARs, upload certificates, collaborate on investigations | Secure external portal, scoped records, document exchange, reminders, e-signature where needed |
| Executive | Monitor quality risk, cost of poor quality, cycle times | KPI dashboards, trend analytics, predictive risk, management review packs |
| Tenant Admin / IT | Configure users, roles, integrations, workflows, retention | Admin console, safe configuration versioning, validation impact, tenant operations |

## 5. Compliance And Regulatory Design

QAMS must support life sciences and medtech first. It should be usable by customers operating under FDA QMSR, ISO 13485, ISO 9001, 21 CFR Part 11, EU Annex 11, GxP expectations, and CSA/GAMP-aligned computerized system assurance practices.

### 5.1 Compliance Control Matrix

| Control area | Platform capability |
| --- | --- |
| 21 CFR Part 11 electronic records | Unique user identity, secure access, audit trails, record retention, accurate copies, system validation evidence |
| 21 CFR Part 11 e-signatures | Reauthentication or MFA proof, signature meaning, reason, signer identity, UTC timestamp, signature/record linking |
| FDA QMSR / ISO 13485 | Control library, evidence mapping, procedure-to-system traceability, management review, complaint/CAPA/risk links |
| ISO 9001 | Process control, customer complaints, corrective action, document control, internal audit, improvement metrics |
| EU Annex 11 | Authorized access, audit trails, backup/restore, supplier assessment, incident/change management, data integrity |
| CSA / GAMP practices | Risk-based validation, automated test evidence, intended-use records, release evidence packs, supplier assessment |
| Data integrity | ALCOA+ evidence model: attributable, legible, contemporaneous, original, accurate, complete, consistent, enduring, available |

### 5.2 Validation Evidence Model

Every production release must generate a validation evidence pack:

- Intended use and system risk assessment
- User requirements and regulatory control mapping
- Functional specifications for regulated workflows
- Configuration inventory and approved workflow/form versions
- Automated test results, exploratory test notes, traceability matrix
- Security test summary and access-control evidence
- Data migration verification where applicable
- Known issues, residual risk, release approval, and rollback plan

The Validation Evidence service owns evidence metadata and links artifacts to releases, controls, tenants, workflows, and test runs.

## 6. Functional Scope

The first production release targets full module breadth. Modules should share platform services instead of creating one-off workflow, audit, signature, file, notification, and reporting logic.

### 6.1 Platform Modules

| Module | Capabilities |
| --- | --- |
| Identity and Tenant | Tenant provisioning, tenant hierarchy, users, groups, sites, roles, permissions, SSO, external users |
| Policy / RBAC / ABAC | Role policies, attribute policies, site/product/supplier scoping, segregation of duties |
| Audit Ledger | Append-only business and system audit events, hash chaining, tamper detection, export |
| E-Signature | Signature ceremonies, signed meaning, reason, reauth proof, record hash linking |
| Workflow | Versioned workflows, stages, routing, parallel/sequential approvals, SLAs, escalations |
| Form Designer | Versioned forms, required fields, conditional sections, controlled lists, validation rules |
| Rules Engine | Event-triggered automation, impact assessment, training triggers, risk escalation |
| Notification | Email, in-app, Teams/Slack-ready channels, escalation reminders, digest notifications |
| Reporting | Operational dashboards, audit packs, management review packs, scheduled reports |
| Search | Keyword, semantic, vector, filters, security trimming, synonym and controlled vocabulary |
| Integration | ERP/MES/LIMS/PLM/CRM/Office/SharePoint/Teams adapters, webhooks, import/export |
| AI Governance | Prompt templates, model routing, grounding, citations, approval gates, audit logs |
| Validation Evidence | Requirements, controls, traceability, test evidence, release validation packs |

### 6.2 Domain Modules

| Module | Production capabilities |
| --- | --- |
| Document Control | SOP/policy/work-instruction lifecycle, versioning, review/approval, effective dates, periodic review, obsolescence, controlled copies |
| Training | Role/job-based curriculum, document-linked training, exams, competency, retraining on document changes, overdue escalation |
| Change Control | Change request, impact assessment, affected documents/products/training/risks/suppliers, approval and implementation tasks |
| Quality Events | Unified intake for deviations, incidents, observations, audit findings, customer issues, supplier issues |
| Nonconformance / Deviation | Classification, containment, disposition, investigation, material/product impact, linked CAPA |
| CAPA | Problem statement, investigation, RCA, action plan, implementation, effectiveness verification, closure |
| Audit / Inspection | Audit program, schedule, checklists, mobile/offline execution, findings, evidence, reports, follow-up |
| Complaints / Post-Market | Complaint intake, triage, investigation, reportability assessment, product links, CAPA/change triggers |
| Supplier Quality / SCAR | Supplier qualification, certificates, audits, incoming issues, scorecards, SCAR, supplier portal |
| Risk / Design Controls | Risk files, hazards, harms, controls, requirements, verification links, traceability gaps |
| Product / Device Master | Product, device, lot/batch, BOM/reference data, market/region metadata, affected-product tracing |
| Equipment / Calibration | Asset inventory, calibration schedules, maintenance, out-of-tolerance impact, certificates |
| Management Review | KPI packs, CAPA effectiveness, audit results, complaint trends, supplier performance, actions |

## 7. Target Architecture

### 7.1 Frontend

- Micro-frontend shell hosted on Azure Static Web Apps behind Azure Front Door and WAF.
- Role-based portals: Admin, Quality Operations, Auditor, Supplier/Customer, Executive.
- Offline-capable PWA/native shell for audits, inspections, frontline reporting, and evidence capture.
- Shared design system with accessible components, dense operational views, fast work queues, and no marketing-style landing page.

### 7.2 Backend

- .NET 10 LTS services on AKS.
- BFF per persona to shape UX-specific APIs and reduce frontend complexity.
- Azure API Management for gateway, JWT validation, throttling, API versioning, developer portal, and external API products.
- Service Bus for commands, reliable workflow messages, outbox/inbox, retries, dead-letter handling.
- Event Grid for external integrations and event fan-out.
- OpenTelemetry instrumentation across API, BFF, services, messaging, and background jobs.

### 7.3 Bounded Contexts

| Bounded context | Service ownership | Primary storage |
| --- | --- | --- |
| Tenant and Identity | Tenant, users, roles, groups, sites | Azure SQL |
| Policy | RBAC/ABAC policies, SoD rules | Azure SQL / Redis cache |
| Workflow and Forms | Workflow definitions, form definitions, work items | Azure SQL / Cosmos DB |
| Audit Ledger | Immutable audit records and hash chain | Azure SQL append-only tables / Blob archive |
| Documents | Document metadata, versions, approvals, controlled copies | Azure SQL / Blob Storage / AI Search |
| Training | Curriculum, assignments, exams, competency | Azure SQL |
| Quality Events | Events, deviations, NCs, investigations | Azure SQL |
| CAPA | CAPA cases, actions, effectiveness checks | Azure SQL |
| Audit / Inspection | Programs, checklists, findings, evidence | Azure SQL / Blob Storage |
| Complaints | Complaint intake, triage, reportability, investigations | Azure SQL |
| Supplier Quality | Suppliers, audits, certificates, SCARs, scorecards | Azure SQL |
| Risk and Design Controls | Hazards, risk controls, requirements, verification | Azure SQL / graph projection |
| Product / Device Master | Products, devices, lots, BOM/reference links | Azure SQL |
| Equipment / Calibration | Assets, schedules, certificates, out-of-tolerance | Azure SQL / Blob Storage |
| Reporting and Analytics | Projections, KPI marts, management packs | Lakehouse / warehouse |
| Search | Indexes, embeddings, semantic search | Azure AI Search |
| AI Assistant | Conversations, prompts, model output logs | Cosmos DB / Blob Storage |
| Validation Evidence | Requirements, controls, traceability, evidence packs | Azure SQL / Blob Storage |

## 8. Public API Standards

### 8.1 Required Headers

```http
Authorization: Bearer <token>
X-Tenant-Id: <tenant-id>
X-Correlation-Id: <uuid>
Idempotency-Key: <uuid-or-client-key>
X-Actor-Context: <interactive|delegated|service|supplier>
Accept: application/json
Content-Type: application/json
```

### 8.2 Response Envelope

```json
{
  "success": true,
  "data": {},
  "errors": [],
  "warnings": [],
  "correlationId": "uuid",
  "apiVersion": "v1"
}
```

### 8.3 Core API Resources

- `/api/v1/quality-events`
- `/api/v1/capa-cases`
- `/api/v1/documents`
- `/api/v1/training-assignments`
- `/api/v1/audits`
- `/api/v1/inspections`
- `/api/v1/complaints`
- `/api/v1/suppliers`
- `/api/v1/products`
- `/api/v1/risks`
- `/api/v1/esignatures`
- `/api/v1/workflow-definitions`
- `/api/v1/form-definitions`
- `/api/v1/evidence`
- `/api/v1/regulatory-controls`
- `/api/v1/ai/assistants`
- `/api/v1/admin/tenants`

### 8.4 API Rules

- All mutating endpoints must accept an idempotency key.
- All regulated actions must create audit ledger entries.
- All approvals and e-signatures must link to immutable record hashes.
- APIs must be tenant-scoped by token claims and `X-Tenant-Id`; mismatches are rejected.
- Version-breaking changes require a new API version and migration policy.

## 9. Event And Messaging Design

### 9.1 Event Envelope

```json
{
  "eventId": "uuid",
  "eventType": "Qams.Capa.Created",
  "schemaVersion": "1.0.0",
  "tenantId": "tenant-123",
  "actorId": "user-456",
  "occurredAtUtc": "2026-05-03T00:00:00Z",
  "correlationId": "uuid",
  "causationId": "uuid",
  "dataClassification": "regulated-confidential",
  "payload": {}
}
```

### 9.2 Required Event Patterns

- Transactional outbox in each service for events created with state changes.
- Inbox/idempotency tracking in every consuming service.
- Dead-letter queues with operational triage dashboards.
- Event schema registry and compatibility checks in CI.
- Event replay only through controlled operational procedures with audit logging.

### 9.3 Initial Event Catalog

- `Qams.Document.Approved`
- `Qams.Document.Effective`
- `Qams.Document.RevisionRequested`
- `Qams.Training.AssignmentCreated`
- `Qams.QualityEvent.Created`
- `Qams.Nonconformance.Confirmed`
- `Qams.Capa.Created`
- `Qams.Capa.EffectivenessCheckDue`
- `Qams.Capa.Closed`
- `Qams.Audit.FindingCreated`
- `Qams.Complaint.TriageCompleted`
- `Qams.Supplier.ScarIssued`
- `Qams.Change.ImpactAssessmentCompleted`
- `Qams.Risk.ScoreChanged`
- `Qams.Esignature.Completed`
- `Qams.Validation.ReleasePackApproved`

## 10. E-Signature And Audit Ledger Design

### 10.1 E-Signature Record

```json
{
  "signatureId": "uuid",
  "tenantId": "tenant-123",
  "recordType": "CapaCase",
  "recordId": "capa-123",
  "recordVersion": "7",
  "signedMeaning": "Approved CAPA closure",
  "reason": "Effectiveness verified and all actions completed",
  "signerUserId": "user-456",
  "signerDisplayName": "Asha Patel",
  "reauthMethod": "mfa",
  "reauthReference": "auth-event-789",
  "signedAtUtc": "2026-05-03T00:00:00Z",
  "recordHash": "sha256:...",
  "previousAuditHash": "sha256:...",
  "auditEntryId": "audit-123"
}
```

### 10.2 Audit Ledger Requirements

- Append-only; no update or delete of committed audit entries.
- Hash chain by tenant and optionally by regulated record.
- UTC timestamps with clear source clock policy.
- Actor, system, IP/device context, before/after summaries, and reason where applicable.
- Exportable human-readable and machine-readable records.
- Tamper detection job with alerting.
- Long-term archive to immutable Blob Storage tiers where required.

## 11. Automation Workflows

The following automated workflows are minimum production scope:

- Document revision automatically identifies affected training, roles, sites, products, suppliers, and open records.
- Approved document revision automatically creates training assignments and blocks use of obsolete controlled copies.
- Audit finding can create a quality event, nonconformance, CAPA, or supplier action based on severity and category.
- Complaint triage can trigger investigation, reportability assessment, risk review, CAPA, or product field action workflow.
- Supplier issue can trigger SCAR, supplier score impact, incoming inspection change, and supplier audit escalation.
- CAPA closure requires action completion, evidence, required signatures, effectiveness criteria, and due-date monitoring.
- Change control requires impact assessment across documents, training, products, risks, suppliers, equipment, and validation evidence.
- Equipment out-of-tolerance event triggers impact assessment against lots, inspections, tests, products, and released records.

## 12. AI Design

### 12.1 AI Use Cases

- QAMS Copilot for question answering over controlled documents and permitted records.
- Quality event and CAPA narrative summarization.
- RCA suggestions using 5 Whys, fishbone, fault tree, and historical similar records.
- CAPA action plan drafting with acceptance criteria suggestions.
- Compliance gap analysis across regulatory controls and tenant evidence.
- SOP translation and simplification suggestions.
- Training exam/question drafting from approved documents.
- Supplier risk insights and trend detection.
- Traceability gap detection across design, risk, test, complaint, and CAPA records.

### 12.2 AI Governance Requirements

- AI outputs are drafts or recommendations until accepted by an authorized human.
- AI cannot approve, sign, close regulated records, or bypass workflow controls.
- Every AI answer must include source citations where grounded content is used.
- Prompt template version, model/version, grounding source IDs, user, tenant, timestamp, and output hash must be audited.
- Sensitive tenant data must remain tenant-isolated.
- AI evaluation suites must test hallucination, privacy leakage, citation quality, unsafe recommendations, and regulated-action boundaries.

## 13. Data Architecture

### 13.1 Storage Strategy

| Data type | Azure service |
| --- | --- |
| Transactional regulated records | Azure SQL Database |
| Flexible workflow/form state and AI conversations | Azure Cosmos DB |
| Controlled documents and evidence files | Azure Blob Storage |
| Cache and policy decisions | Azure Cache for Redis |
| Semantic and keyword search | Azure AI Search |
| Analytics and KPI marts | Microsoft Fabric or Azure Data Lake / Synapse pattern |
| Secrets and keys | Azure Key Vault |

### 13.2 Data Rules

- Database per bounded context; no cross-service database writes.
- Tenant ID is mandatory on every tenant-owned record.
- Soft delete is not sufficient for regulated records; records move through controlled lifecycle states.
- Retention and legal hold policies are tenant-configurable but governed by compliance constraints.
- Search indexes must enforce security trimming and tenant isolation.
- Analytics projections are derived; source-of-record remains in bounded context services.

## 14. Multi-Tenancy

QAMS will support hybrid tenancy:

- Standard tenants: shared infrastructure, shared database per service, tenant ID isolation, row-level security where appropriate.
- Enterprise tenants: dedicated database per regulated service where required, optional dedicated storage accounts, private networking, and customer-managed keys.
- Regulated enterprise tenants: stricter validation pack, controlled configuration promotion, dedicated encryption scopes, and optional regional isolation.

Tenant isolation must be tested through automated security tests, data-access tests, and search-index leakage tests.

## 15. Security Architecture

- Microsoft Entra ID and Entra External ID for workforce and external identities.
- MFA and reauthentication for regulated approvals and e-signatures.
- RBAC plus ABAC for role, site, product, supplier, and record-state constraints.
- Managed identities for service-to-service access.
- Key Vault for secrets, certificates, signing keys, and encryption keys.
- Private endpoints for data services.
- Azure Front Door WAF and API Management throttling.
- Microsoft Defender for Cloud and container/image scanning.
- Security event monitoring, access reviews, and privileged access management.
- Segregation-of-duties policy checks for critical workflows.

## 16. Observability And Operations

### 16.1 Telemetry

- OpenTelemetry traces, metrics, and logs.
- Correlation ID propagation from frontend to BFF, API gateway, services, queues, and background jobs.
- Business KPIs as first-class metrics: CAPA cycle time, overdue training, audit findings, complaint aging, supplier score, document review aging.

### 16.2 SLO Targets

| Capability | Target |
| --- | --- |
| Core API availability | 99.9% for standard, 99.95% for enterprise tier |
| p95 read API latency | Under 500 ms for common work queue and record reads |
| p95 write API latency | Under 900 ms excluding long-running workflow automation |
| Search freshness | Under 5 minutes for regulated record metadata |
| Notification dispatch | Under 2 minutes for high-priority workflow tasks |
| RPO | 15 minutes standard, 5 minutes enterprise |
| RTO | 4 hours standard, 1 hour enterprise |

## 17. Integration Architecture

Initial integration targets:

- Microsoft 365 / Office for controlled document coauthoring and source files.
- SharePoint and Teams for document migration, notifications, and collaboration links.
- ERP for suppliers, products, lots, purchase orders, and material disposition.
- MES/LIMS for inspections, lab results, deviations, out-of-spec events, and batch context.
- PLM for design controls, requirements, BOM, change objects, and product lifecycle.
- CRM/customer support for complaints and customer quality events.
- Email ingestion for complaint/supplier intake where needed.

Integrations must use API Management products, webhooks/Event Grid, queued imports, idempotent upserts, validation reports, and reconciliation dashboards.

## 18. UX Design Requirements

- First screen after login is a role-based workbench, not a landing page.
- Common user flows must be task-oriented: "My work", "Due soon", "At risk", "Needs signature", "Recently changed".
- Every regulated record has a timeline, linked records, evidence, audit trail, workflow state, and signatures.
- Admin configuration screens must show draft/approved/effective versions and validation impact.
- Mobile/offline screens must prioritize short forms, evidence capture, checklist progress, QR/barcode access, and conflict-safe sync.
- Executive dashboards must show trends, risk, cycle time, overdue work, cost of poor quality, and management review readiness.

## 19. Acceptance Criteria

The design is production-ready when:

- All core modules can create, route, approve/sign, audit, report, and export regulated records.
- A document revision can automatically trigger impact assessment and training.
- An audit finding, complaint, nonconformance, or supplier issue can trigger CAPA.
- CAPA cannot close without required actions, evidence, effectiveness check, and signatures.
- AI recommendations are traceable, cited, permission-filtered, and human-approved.
- Regulatory controls can be mapped to procedures, system features, and evidence.
- Validation evidence packs can be generated for releases and tenant configuration changes.
- Tenant isolation, audit ledger tamper detection, backup/restore, DR, and offline sync are tested.

## 20. Risks And Mitigations

| Risk | Mitigation |
| --- | --- |
| Full module breadth delays launch | Build shared platform primitives first; deliver modules on common workflow/forms/audit/signature services |
| AI creates compliance risk | Human approval, citations, no autonomous closure, audited prompts/outputs, evaluations |
| Over-configurability weakens validation | Versioned configurations, approval workflow, impact assessment, validation evidence |
| Tenant isolation failure | Defense-in-depth: claims, policy service, row-level checks, search trimming, automated leakage tests |
| Event-driven complexity | Outbox/inbox, schema registry, replay procedures, dead-letter operations |
| Regulated customers require evidence | Validation Evidence service and release evidence packs from the start |

## 21. Source References

- Veeva QMS: https://www.veeva.com/products/veeva-qms/
- MasterControl Quality: https://www.mastercontrol.com/quality/
- Greenlight Guru QMS: https://www.greenlight.guru/quality-management-software
- Qualio: https://www.qualio.com/
- Ideagen QMS: https://www.ideagen.com/solutions/quality/quality-management
- Dot Compliance: https://www.dotcompliance.com/
- ComplianceQuest: https://www.compliancequest.com/
- AssurX: https://www.assurx.com/quality-management-software/
- FDA QMSR: https://www.fda.gov/medical-devices/postmarket-requirements-devices/quality-management-system-regulation-qmsr
- FDA CSA Guidance: https://www.fda.gov/regulatory-information/search-fda-guidance-documents/computer-software-assurance-production-and-quality-management-system-software
- 21 CFR Part 11: https://www.ecfr.gov/current/title-21/chapter-I/subchapter-A/part-11
- ISO 9001: https://www.iso.org/standard/62085.html
- ISO 13485: https://www.iso.org/standard/59752.html
- EU EudraLex Annex 11: https://health.ec.europa.eu/medicinal-products/eudralex/eudralex-volume-4_en
- .NET support policy: https://dotnet.microsoft.com/en-us/platform/support/policy
