# QAMS Architecture Diagrams

Status: Source-controlled architecture baseline
Primary market: Life sciences and medtech
Related documents:
- `QAMS_Azure_Architecture.md`
- `QAMS_Detailed_Blueprint.md`
- `QAMS_Final_Design_Document.md`
- `QAMS_Implementation_Roadmap.md`

## 1. C4-Style System Context

```mermaid
flowchart LR
    qa[QA Manager]
    qe[Quality Engineer]
    op[Operator / Frontline User]
    aud[Auditor / Inspector]
    sup[Supplier / Partner]
    exec[Executive]
    admin[Tenant Admin / IT]

    qams[QAMS SaaS Platform]

    entra[Microsoft Entra ID / External ID]
    m365[Microsoft 365 / Office / SharePoint / Teams]
    erp[ERP]
    mes[MES / LIMS]
    plm[PLM]
    crm[CRM / Customer Support]
    email[Email Ingestion]
    regs[Regulatory Standards and Controls]

    qa --> qams
    qe --> qams
    op --> qams
    aud --> qams
    sup --> qams
    exec --> qams
    admin --> qams

    qams --> entra
    qams --> m365
    qams --> erp
    qams --> mes
    qams --> plm
    qams --> crm
    email --> qams
    regs --> qams
```

## 2. Azure Container Architecture

```mermaid
flowchart TB
    subgraph Edge["Azure Edge"]
        afd[Azure Front Door + WAF]
        apim[Azure API Management]
    end

    subgraph Web["Frontend"]
        swa[Azure Static Web Apps]
        shell[Micro-Frontend Shell]
        adminPortal[Admin Portal]
        opsPortal[Quality Operations Portal]
        auditorPortal[Auditor Portal]
        supplierPortal[Supplier Portal]
        execPortal[Executive Portal]
        mobile[Offline PWA / Mobile Shell]
    end

    subgraph BFF["Backend For Frontend"]
        adminBff[Admin BFF]
        opsBff[Operations BFF]
        auditorBff[Auditor BFF]
        supplierBff[Supplier BFF]
        execBff[Executive BFF]
        mobileBff[Mobile Sync BFF]
    end

    subgraph AKS["AKS: .NET 10 Microservices"]
        platform[Platform Services]
        domains[Domain Services]
        ai[AI And Search Services]
        integrations[Integration Services]
        workers[Background Workers]
    end

    subgraph Messaging["Messaging"]
        sb[Azure Service Bus]
        eg[Azure Event Grid]
    end

    subgraph Data["Data Platform"]
        sql[Azure SQL Databases]
        cosmos[Azure Cosmos DB]
        blob[Azure Blob Storage]
        redis[Azure Cache For Redis]
        search[Azure AI Search]
        lake[Lakehouse / Warehouse]
        kv[Azure Key Vault]
    end

    subgraph Ops["Security And Operations"]
        monitor[Azure Monitor]
        appi[Application Insights]
        logs[Log Analytics]
        defender[Microsoft Defender For Cloud]
    end

    users[Users] --> afd
    afd --> swa
    swa --> shell
    shell --> adminPortal
    shell --> opsPortal
    shell --> auditorPortal
    shell --> supplierPortal
    shell --> execPortal
    shell --> mobile

    adminPortal --> adminBff
    opsPortal --> opsBff
    auditorPortal --> auditorBff
    supplierPortal --> supplierBff
    execPortal --> execBff
    mobile --> mobileBff

    adminBff --> apim
    opsBff --> apim
    auditorBff --> apim
    supplierBff --> apim
    execBff --> apim
    mobileBff --> apim

    apim --> platform
    apim --> domains
    apim --> ai
    apim --> integrations

    platform <--> sb
    domains <--> sb
    workers <--> sb
    domains --> eg
    integrations --> eg

    platform --> sql
    platform --> cosmos
    platform --> redis
    platform --> kv
    domains --> sql
    domains --> blob
    domains --> search
    domains --> lake
    ai --> search
    ai --> cosmos
    ai --> blob
    integrations --> sql
    integrations --> sb

    platform --> monitor
    domains --> monitor
    ai --> appi
    integrations --> logs
    AKS --> defender
```

## 3. Service Topology

```mermaid
flowchart LR
    subgraph Platform["Regulated Platform Services"]
        tenant[Identity + Tenant]
        policy[Policy RBAC / ABAC]
        audit[Audit Ledger]
        esig[E-Signature]
        workflow[Workflow]
        forms[Form Designer]
        rules[Rules Engine]
        notify[Notification]
        report[Reporting]
        evidence[Validation Evidence]
    end

    subgraph Domain["QMS Domain Services"]
        docs[Document Control]
        training[Training]
        change[Change Control]
        qe[Quality Events]
        nc[Nonconformance / Deviation]
        capa[CAPA]
        audits[Audit / Inspection]
        complaints[Complaints / Post-Market]
        supplier[Supplier Quality / SCAR]
        risk[Risk / Design Controls]
        product[Product / Device Master]
        equipment[Equipment / Calibration]
        review[Management Review]
    end

    subgraph Intelligence["Intelligence Services"]
        search[Search]
        ai[QAMS Copilot]
        aigov[AI Governance]
        analytics[Analytics]
    end

    subgraph External["External Systems"]
        m365[Microsoft 365]
        erp[ERP]
        mes[MES / LIMS]
        plm[PLM]
        crm[CRM]
    end

    tenant --> policy
    policy --> docs
    policy --> training
    policy --> capa
    policy --> supplier

    workflow --> docs
    workflow --> training
    workflow --> qe
    workflow --> capa
    workflow --> audits
    workflow --> complaints
    workflow --> supplier
    workflow --> change

    forms --> workflow
    rules --> workflow
    rules --> training
    rules --> change
    rules --> capa

    docs --> training
    docs --> change
    qe --> nc
    qe --> capa
    audits --> qe
    complaints --> qe
    complaints --> risk
    supplier --> qe
    supplier --> capa
    change --> docs
    change --> training
    change --> risk
    risk --> product
    equipment --> qe
    review --> analytics

    docs --> audit
    training --> audit
    qe --> audit
    capa --> audit
    audits --> audit
    complaints --> audit
    supplier --> audit
    change --> audit
    risk --> audit
    esig --> audit

    docs --> search
    qe --> search
    capa --> search
    audits --> search
    supplier --> search
    risk --> search

    search --> ai
    aigov --> ai
    ai --> audit
    analytics --> review
    evidence --> review

    m365 <--> docs
    erp <--> supplier
    erp <--> product
    mes <--> equipment
    mes <--> qe
    plm <--> risk
    plm <--> product
    crm <--> complaints
```

## 4. Closed-Loop Quality Event Flow

```mermaid
flowchart TD
    intake[Quality Event Intake]
    classify[Classify Event Type And Severity]
    containment[Containment / Immediate Action]
    investigate[Investigation Workspace]
    rca[Root Cause Analysis]
    decision{CAPA Required?}
    capa[Create CAPA]
    actions[Corrective / Preventive Actions]
    implement[Implement Actions]
    verify[Effectiveness Verification]
    close[Regulated Closure + E-Signature]
    review[Management Review And Analytics]

    docImpact[Document Impact]
    trainingImpact[Training Impact]
    riskImpact[Risk Impact]
    supplierImpact[Supplier Impact]
    productImpact[Product / Device Impact]
    evidence[Evidence Pack]
    audit[Immutable Audit Ledger]
    ai[AI Summary / RCA Suggestions]

    intake --> classify
    classify --> containment
    containment --> investigate
    investigate --> ai
    investigate --> rca
    rca --> decision
    decision -- Yes --> capa
    decision -- No --> close
    capa --> actions
    actions --> implement
    implement --> verify
    verify --> close
    close --> review

    investigate --> docImpact
    investigate --> trainingImpact
    investigate --> riskImpact
    investigate --> supplierImpact
    investigate --> productImpact
    docImpact --> evidence
    trainingImpact --> evidence
    riskImpact --> evidence
    supplierImpact --> evidence
    productImpact --> evidence
    evidence --> close

    intake --> audit
    classify --> audit
    containment --> audit
    investigate --> audit
    rca --> audit
    capa --> audit
    actions --> audit
    verify --> audit
    close --> audit
```

## 5. Document Revision To Training Automation

```mermaid
sequenceDiagram
    participant Owner as Document Owner
    participant Docs as Document Control Service
    participant Workflow as Workflow Service
    participant Rules as Rules Engine
    participant Training as Training Service
    participant Notify as Notification Service
    participant Esig as E-Signature Service
    participant Audit as Audit Ledger

    Owner->>Docs: Submit document revision
    Docs->>Workflow: Start review workflow
    Workflow->>Audit: Record review route
    Workflow->>Esig: Request approval signatures
    Esig->>Audit: Record signature and record hash
    Workflow->>Docs: Mark approved with effective date
    Docs->>Rules: Publish Document.Approved event
    Rules->>Training: Create retraining assignments
    Rules->>Notify: Notify affected users and managers
    Training->>Audit: Record assignment creation
    Notify->>Audit: Record notification dispatch
```

## 6. API And Event Flow

```mermaid
sequenceDiagram
    participant UI as Micro-Frontend / Mobile
    participant BFF as Persona BFF
    participant APIM as API Management
    participant Service as Domain Service
    participant DB as Service Database
    participant Outbox as Transactional Outbox
    participant Bus as Service Bus
    participant Worker as Consumer Service
    participant Audit as Audit Ledger

    UI->>BFF: Mutating request with tenant, correlation, idempotency
    BFF->>APIM: Forward validated request
    APIM->>Service: Route to versioned API
    Service->>Service: Validate policy and command
    Service->>DB: Save state change
    Service->>Outbox: Save event in same transaction
    Service->>Audit: Append audit record
    Service-->>APIM: Response envelope
    APIM-->>BFF: Response
    BFF-->>UI: Shaped response
    Outbox->>Bus: Publish event
    Bus->>Worker: Deliver event
    Worker->>Worker: Inbox idempotency check
    Worker->>Audit: Append consumer audit record
```

## 7. AI Governance Flow

```mermaid
flowchart TD
    user[Authorized User]
    copilot[QAMS Copilot]
    policy[Policy And Tenant Filter]
    search[Azure AI Search With Security Trimming]
    records[Permitted QMS Records]
    docs[Controlled Documents]
    prompt[Versioned Prompt Template]
    model[Azure OpenAI Model]
    output[Draft Answer / Recommendation]
    citations[Source Citations]
    human[Human Review And Acceptance]
    action[Regulated Workflow Action]
    ailog[AI Output Audit Log]
    audit[Audit Ledger]

    user --> copilot
    copilot --> policy
    policy --> search
    search --> records
    search --> docs
    copilot --> prompt
    prompt --> model
    search --> model
    model --> output
    output --> citations
    output --> ailog
    citations --> user
    output --> human
    human --> action
    action --> audit
    ailog --> audit

    output -. cannot approve, close, or sign .-> human
```

## 8. Data And Compliance Architecture

```mermaid
flowchart LR
    subgraph Sources["Record Sources"]
        docs[Documents]
        capa[CAPA]
        audits[Audits]
        complaints[Complaints]
        supplier[Suppliers]
        risk[Risk / Design]
        equipment[Equipment]
    end

    subgraph Stores["Source Stores"]
        sql[Azure SQL Per Bounded Context]
        blob[Versioned Blob Storage]
        cosmos[Cosmos DB For Flexible State]
    end

    subgraph Evidence["Compliance Evidence"]
        ledger[Append-Only Audit Ledger]
        hash[Hash Chain]
        esig[E-Signature Records]
        validation[Validation Evidence Packs]
        retention[Retention / Legal Hold]
    end

    subgraph Intelligence["Derived Stores"]
        search[AI Search Indexes]
        lake[Analytics Lakehouse]
        reports[Reports And Dashboards]
    end

    docs --> sql
    capa --> sql
    audits --> sql
    complaints --> sql
    supplier --> sql
    risk --> sql
    equipment --> sql
    docs --> blob
    audits --> blob
    complaints --> blob
    capa --> blob
    docs --> cosmos

    sql --> ledger
    blob --> ledger
    cosmos --> ledger
    ledger --> hash
    esig --> ledger
    validation --> ledger
    retention --> sql
    retention --> blob

    sql --> search
    blob --> search
    sql --> lake
    lake --> reports
    search --> reports
```

## 9. Deployment Environments

```mermaid
flowchart LR
    dev[Dev]
    qa[QA]
    uat[UAT / Validation]
    prod[Production]

    repo[Source Repository]
    ci[CI: Build, Test, Scan]
    images[Container Registry]
    iac[IaC: Bicep]
    helm[Helm Deploy]
    evidence[Validation Evidence Pack]
    gates[Release Gates]

    repo --> ci
    ci --> images
    ci --> evidence
    iac --> dev
    images --> helm
    helm --> dev
    dev --> qa
    qa --> uat
    uat --> gates
    evidence --> gates
    gates --> prod
```

## 10. Roadmap Overview

```mermaid
gantt
    title QAMS Production Roadmap
    dateFormat  YYYY-MM-DD
    axisFormat  %b

    section Foundation
    Phase 0 Product + Compliance Foundation :p0, 2026-05-01, 4w
    Phase 1 SaaS Platform Foundation :p1, after p0, 8w

    section Core QMS
    Phase 2 Full Core QMS Breadth :p2, after p1, 16w

    section Differentiation
    Phase 3 AI + Automation :p3, after p2, 10w
    Phase 4 Enterprise Readiness :p4, after p3, 12w

    section Launch
    Phase 5 Production Hardening + Launch :p5, after p4, 8w
```
