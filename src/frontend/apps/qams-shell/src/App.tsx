import { AlertTriangle, CheckCircle2, ClipboardCheck, FileText, Search, ShieldCheck, Signature, Workflow } from "lucide-react";

const workItems = [
  { label: "CAPA awaiting QA approval", module: "CAPA", priority: "High", due: "Today" },
  { label: "Document revision training impact", module: "Documents", priority: "Medium", due: "Tomorrow" },
  { label: "Supplier SCAR response overdue", module: "Supplier", priority: "High", due: "2 days" },
  { label: "Audit finding needs RCA", module: "Audit", priority: "Medium", due: "5 days" }
];

const metrics = [
  { label: "Open CAPA", value: "18", trend: "-12%" },
  { label: "Overdue Training", value: "42", trend: "-8%" },
  { label: "Audit Findings", value: "11", trend: "+3%" },
  { label: "Supplier Risk", value: "7", trend: "-2%" }
];

export function App() {
  return (
    <main className="app-shell">
      <aside className="sidebar" aria-label="Primary navigation">
        <div className="brand">
          <ShieldCheck size={28} />
          <span>QAMS</span>
        </div>
        <nav>
          <a className="active" href="#"><ClipboardCheck size={18} /> Workbench</a>
          <a href="#"><FileText size={18} /> Documents</a>
          <a href="#"><Workflow size={18} /> Workflows</a>
          <a href="#"><Signature size={18} /> E-Signatures</a>
          <a href="#"><Search size={18} /> Search</a>
        </nav>
      </aside>

      <section className="workspace">
        <header className="topbar">
          <div>
            <p>Quality Operations</p>
            <h1>Regulated workbench</h1>
          </div>
          <button type="button"><AlertTriangle size={18} /> Review risks</button>
        </header>

        <section className="metrics" aria-label="Quality metrics">
          {metrics.map((metric) => (
            <article key={metric.label}>
              <span>{metric.label}</span>
              <strong>{metric.value}</strong>
              <small>{metric.trend}</small>
            </article>
          ))}
        </section>

        <section className="content-grid">
          <div className="panel">
            <div className="panel-heading">
              <h2>My regulated work</h2>
              <CheckCircle2 size={20} />
            </div>
            <div className="work-list">
              {workItems.map((item) => (
                <button key={item.label} type="button" className="work-item">
                  <span>
                    <strong>{item.label}</strong>
                    <small>{item.module}</small>
                  </span>
                  <span className={item.priority === "High" ? "badge high" : "badge"}>{item.due}</span>
                </button>
              ))}
            </div>
          </div>

          <div className="panel">
            <div className="panel-heading">
              <h2>Automation queue</h2>
              <Workflow size={20} />
            </div>
            <ul className="automation-list">
              <li>Document revision created 24 retraining assignments</li>
              <li>Complaint triage recommended risk review</li>
              <li>CAPA closure blocked until effectiveness evidence is attached</li>
              <li>Supplier scorecard updated from latest SCAR trend</li>
            </ul>
          </div>
        </section>
      </section>
    </main>
  );
}
