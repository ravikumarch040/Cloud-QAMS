# QAMS Implementation Roadmap Checklist

## Current Completed Work
- [x] Implement E-Signature service with create and list APIs
- [x] Implement Policy service with RBAC evaluation
- [x] Integrate CAPA closure with e-signature creation
- [x] Update README with current implementation status
- [x] Implement Notification service for in-app and email notifications

## Phase 0: Product and Compliance Foundation
- [ ] Convert the product vision into implementable requirements
- [ ] Lock domain boundaries and module scope
- [ ] Establish compliance, validation, security, and UX baselines
- [ ] Produce architecture diagrams, data-flow diagrams, and threat model
- [ ] Produce regulatory control matrix for FDA QMSR, ISO 13485, ISO 9001, Part 11, Annex 11
- [ ] Produce user requirement specification for the first production release
- [ ] Produce validation strategy aligned to CSA/GAMP practices
- [ ] Produce UX information architecture for all workbenches
- [ ] Define release definition of done and validation evidence pack template
- [ ] Assign module owners and define core workflows, APIs, events, reports, and acceptance criteria
- [ ] Identify regulated actions requiring e-signature
- [ ] Document tenant isolation, retention, legal hold, and audit trail rules
- [ ] Prepare build backlog for Phase 1

## Phase 1: SaaS Platform Foundation
- [ ] Build Azure cloud foundation with subscriptions, RGs, AKS, API gateway, WAF, Key Vault, private networking, and managed identities
- [ ] Build CI/CD and IaC with Bicep modules, Helm charts, environment promotion, container builds, security scans, and deployment gates
- [ ] Build identity and tenant foundation: tenant provisioning, Entra ID integration, external identity support, tenant settings, and site hierarchy
- [ ] Build policy engine for RBAC/ABAC, permission checks, segregation of duties, and policy cache
- [ ] Build audit ledger with append-only audit entries, hash chain, audit export, and tamper detection
- [ ] Build e-signature service and ceremony with reauthentication/MFA proof, signed meaning, record hash, and audit link
- [ ] Build workflow/forms/rules engine with versioned workflow definitions, work items, routing, SLAs, escalation, and rule execution
- [ ] Build notifications for in-app, email, digest, overdue reminders, and escalation
- [ ] Build observability with OpenTelemetry, App Insights, Log Analytics, dashboards, and alerts
- [ ] Build validation evidence service and release pack generation
- [ ] Build frontend shell with micro-frontend layout, navigation, and shared auth
- [ ] Enable sample regulated workflow: route, approve, e-sign, audit, notify, and export evidence
- [ ] Deploy dev, qa, uat, and prod-like environments through CI/CD
- [ ] Meet baseline security and observability requirements for platform services
- [ ] Generate Phase 1 validation evidence pack

## Phase 2: Full Core QMS Breadth
- [ ] Deliver Document Control and Training modules with approval, controlled copy update, training assignment, and retraining on revision
- [ ] Deliver Quality Events, Nonconformance, Deviation, CAPA modules with intake, classification, containment, RCA, CAPA creation, action tracking, and effectiveness verification
- [ ] Deliver Audit and Inspection modules with audit findings, CAPA linkage, mobile checklist, and report generation
- [ ] Deliver Complaint and Supplier Quality modules with complaint triage, investigation, supplier risk score, SCAR, and certificate expiry reminders
- [ ] Deliver Product Master, Risk/Design Controls, and Change Control modules with impact assessment across products, risks, docs, training, suppliers, equipment, and validation evidence
- [ ] Deliver Equipment/Calibration, Management Review, and Reporting modules with out-of-tolerance assessment, KPI generation, and review actions
- [ ] Implement document revision ➜ training assignment and obsolete version blocking
- [ ] Implement audit finding ➜ CAPA creation and evidence linkage
- [ ] Implement complaint investigation ➜ risk review ➜ CAPA/change workflow
- [ ] Implement supplier nonconformance ➜ SCAR creation and scorecard impact
- [ ] Implement change control impact checks across documents, products, risks, suppliers, equipment, training, and validation evidence
- [ ] Support create/read/update workflow actions, audit trail, permissions, reporting, search indexing, and export for all modules
- [ ] Pass automated and UAT tests for required cross-module scenarios
- [ ] Generate Phase 2 validation evidence pack

## Phase 3: AI and Automation Differentiators
- [ ] Add QAMS Copilot over controlled documents, permitted records, and regulatory evidence
- [ ] Add semantic search with tenant isolation and permission trimming
- [ ] Add quality event and CAPA summary generation
- [ ] Add RCA suggestions using historical similar records and root cause patterns
- [ ] Add CAPA plan drafting with owner, due date, acceptance criteria, and verification suggestions
- [ ] Add compliance gap analysis across regulatory controls and tenant evidence
- [ ] Add SOP translation and simplified-language suggestions
- [ ] Add training exam/question drafting from approved documents
- [ ] Add supplier risk insights and trend detection
- [ ] Ensure AI cannot approve, sign, close regulated records, or bypass workflow
- [ ] Audit every AI response with citations, prompt/model version, grounding sources, output hash, user, tenant, and timestamp
- [ ] Include AI evaluation reports in validation evidence

## Phase 4: Enterprise Readiness
- [ ] Build external supplier/customer portal with scoped secure access, SCAR responses, certificate uploads, audit responses, and complaint collaboration
- [ ] Build mobile/offline support for audit/inspection checklists, evidence capture, QR/barcode launch, sync queue, and conflict resolution
- [ ] Build integrations for Microsoft 365, SharePoint, Teams, ERP, MES, LIMS, PLM, CRM, email ingestion, and webhook/event products
- [ ] Build data migration capabilities for document import, metadata mapping, user/group import, legacy CAPA/audit/complaint import, and reconciliation reports
- [ ] Build tenant operations for provisioning, environment promotion, config export/import, feature flags, usage, and health dashboards
- [ ] Build enterprise security for customer-managed keys, private connectivity, access reviews, and privileged workflows
- [ ] Build commercial operations for billing/metering, usage reports, and support/admin console
- [ ] Enable external collaboration without unauthorized tenant data exposure
- [ ] Pass mobile/offline conflict, security, and audit tests
- [ ] Deliver at least three production-ready integration patterns: API, webhook/event, and batch import
- [ ] Generate Phase 4 validation evidence pack

## Phase 5: Production Hardening and Launch
- [ ] Perform penetration testing and remediation
- [ ] Review threat model and obtain security signoff
- [ ] Run performance/load testing for core APIs, search, workflow, and reporting
- [ ] Run backup/restore and disaster recovery drills
- [ ] Run tenant isolation and search leakage testing
- [ ] Run audit ledger tamper-detection testing
- [ ] Run offline sync resilience testing
- [ ] Run data migration dry runs
- [ ] Conduct UAT with quality/compliance SMEs
- [ ] Review validation evidence and obtain release approval
- [ ] Publish production runbooks, support procedures, and incident response playbooks
- [ ] Close all critical/high security issues
- [ ] Meet p95 API and search targets under pilot load
- [ ] Complete backup/restore and DR drills with documented results
- [ ] Pass compliance test suite
- [ ] Approve validation evidence pack
- [ ] Provision pilot tenants with approved configurations
- [ ] Activate support and SRE dashboards

## Testing and Validation
- [ ] Build unit tests for domain rules, policy checks, workflow transitions, and validation rules
- [ ] Build contract tests for API schemas, event compatibility, and BFF contracts
- [ ] Build integration tests for persistence, search, and messaging dependencies
- [ ] Build end-to-end tests for document-to-training, audit-to-CAPA, complaint-to-investigation, supplier-to-SCAR flows
- [ ] Build security tests for tenant isolation, RBAC/ABAC, e-signature reauth, authorization, and search trimming
- [ ] Build compliance tests for Part 11 signatures, audit trails, export, retention/legal hold, and validation evidence
- [ ] Build performance tests for work queues, record throughput, workflow automation, search, and reporting
- [ ] Build resilience tests for outbox/inbox recovery, event replay, backup/restore, and offline sync
- [ ] Build AI evaluations for grounding, citations, hallucination, privacy leakage, and unsafe recommendations
- [ ] Produce requirements traceability matrix
- [ ] Produce risk-based test plan
- [ ] Capture automated run records and manual/UAT evidence
- [ ] Produce security, privacy, and AI evaluation reports
- [ ] Produce configuration inventory and release approval evidence

## Production Definition of Done
- [ ] Use shared identity, policy, workflow, audit, e-signature, notification, and validation evidence services where applicable
- [ ] Include API, event, UI, security, and compliance tests
- [ ] Support tenant isolation and site/product/supplier scoping
- [ ] Create audit entries for regulated actions
- [ ] Handle idempotency for mutating APIs and message consumers
- [ ] Emit OpenTelemetry traces, metrics, and logs
- [ ] Provide operational dashboards and alerts for failure modes
- [ ] Include work in release validation evidence
- [ ] Provide user-facing documentation or admin guidance where needed

## Immediate Next Engineering Actions
- [ ] Create repository structure for frontend, services, shared, infra, deploy, docs, and tests
- [ ] Scaffold .NET services with health checks, telemetry, API versioning, validation, outbox/inbox, policy enforcement, and audit client
- [ ] Scaffold frontend shell with auth, role workbench, layout, and micro-frontend strategy
- [ ] Build a platform vertical slice for tenant creation, workflow creation, sample record submission, approval, e-signature, audit, notification, and evidence export
- [ ] Establish CI/CD, IaC, container scanning, test reporting, and validation evidence generation from day one
