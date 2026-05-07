import { pillars } from '../content'
import { FadeIn } from './FadeIn'

export function CapabilityPillars() {
  return (
    <section id="pillars" className="relative px-4 py-24 md:px-8">
      <div className="pointer-events-none absolute inset-x-0 top-0 h-px bg-gradient-to-r from-transparent via-white/15 to-transparent" />
      <div className="mx-auto max-w-6xl">
        <FadeIn>
          <h2 className="font-display text-3xl font-semibold text-white md:text-4xl">
            Everything quality touches — orchestrated
          </h2>
          <p className="mt-4 max-w-2xl text-lg text-slate-400">
            Replace brittle toolchains with modules that share workflows, signatures, audit narrative,
            and automation primitives — so teams spend energy on science and patients, not paperwork
            archaeology.
          </p>
        </FadeIn>

        <div className="mt-14 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {pillars.map((item, i) => (
            <FadeIn key={item.title} delay={(i % 3) * 0.06}>
              <article
                className={`relative h-full overflow-hidden rounded-3xl border border-white/10 bg-gradient-to-br ${item.accent} p-[1px] shadow-lg shadow-black/30`}
              >
                <div className="h-full rounded-[calc(1.5rem-1px)] bg-slate-950/85 p-7 backdrop-blur-sm">
                  <div className="mb-4 h-2 w-12 rounded-full bg-gradient-to-r from-cyan-400 to-teal-400" />
                  <h3 className="font-display text-lg font-semibold text-white">{item.title}</h3>
                  <p className="mt-3 text-sm leading-relaxed text-slate-400">{item.body}</p>
                </div>
              </article>
            </FadeIn>
          ))}
        </div>

        <FadeIn className="mt-14">
          <div className="overflow-hidden rounded-3xl border border-white/10 bg-slate-900/40">
            <div className="grid md:grid-cols-2">
              <img
                src="https://images.unsplash.com/photo-1532187863486-abf9dbad1b69?auto=format&fit=crop&w=1100&q=75"
                alt="Laboratory teamwork"
                width={1100}
                height={720}
                loading="lazy"
                decoding="async"
                className="h-64 w-full object-cover md:h-full md:min-h-[280px]"
              />
              <div className="flex flex-col justify-center p-8 md:p-12">
                <p className="text-xs font-bold uppercase tracking-[0.3em] text-cyan-300/90">
                  Designed for regulated UX
                </p>
                <h3 className="mt-3 font-display text-2xl font-semibold text-white">
                  Work queues, not module dead-ends
                </h3>
                <p className="mt-4 text-slate-400">
                  Operators see tasks and effective documents. Investigators see timelines and linked evidence.
                  Leaders see risk signals — all fed by the same controlled backbone.
                </p>
              </div>
            </div>
          </div>
        </FadeIn>
      </div>
    </section>
  )
}
