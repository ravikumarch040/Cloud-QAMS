import { differentiators } from '../content'
import { FadeIn } from './FadeIn'

export function Differentiators() {
  return (
    <section id="compare" className="relative px-4 py-24 md:px-8">
      <div className="mx-auto max-w-6xl">
        <FadeIn>
          <h2 className="font-display text-3xl font-semibold text-white md:text-4xl">
            Built for how modern eQMS buyers evaluate platforms
          </h2>
          <p className="mt-4 max-w-2xl text-lg text-slate-400">
            Buyers compare breadth, configurability, automation, supplier collaboration, audit readiness,
            and responsible AI. Cloud QAMS targets those expectations directly — without bolt-on glue code.
          </p>
        </FadeIn>

        <div className="mt-14 overflow-hidden rounded-3xl border border-white/10">
          <div className="grid grid-cols-[1.2fr_1fr_1fr] gap-4 bg-slate-900/60 px-4 py-4 text-xs font-bold uppercase tracking-wider text-slate-400 md:px-8">
            <span>Capability</span>
            <span className="hidden md:inline">Typical patchwork</span>
            <span className="hidden md:inline text-teal-300">Cloud QAMS direction</span>
          </div>
          {differentiators.map((row, i) => (
            <FadeIn key={row.capability} delay={i * 0.05}>
              <div className="grid border-t border-white/10 bg-slate-950/40 px-4 py-6 md:grid-cols-[1.2fr_1fr_1fr] md:items-start md:gap-4 md:px-8">
                <p className="font-display text-base font-semibold text-white">{row.capability}</p>
                <p className="mt-2 text-sm text-slate-500 md:mt-0">
                  <span className="mr-2 inline rounded-md bg-white/5 px-2 py-0.5 text-[10px] font-bold uppercase tracking-wide text-slate-400 md:hidden">
                    Typical
                  </span>
                  {row.typical}
                </p>
                <p className="mt-3 text-sm leading-relaxed text-cyan-100 md:mt-0">
                  <span className="mr-2 inline rounded-md bg-teal-500/15 px-2 py-0.5 text-[10px] font-bold uppercase tracking-wide text-teal-300 md:hidden">
                    QAMS
                  </span>
                  {row.qams}
                </p>
              </div>
            </FadeIn>
          ))}
        </div>
      </div>
    </section>
  )
}
