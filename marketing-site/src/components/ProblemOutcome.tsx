import { problems } from '../content'
import { FadeIn } from './FadeIn'

export function ProblemOutcome() {
  return (
    <section id="story" className="relative px-4 py-24 md:px-8">
      <div className="mx-auto max-w-6xl">
        <FadeIn>
          <p className="text-center text-xs font-bold uppercase tracking-[0.35em] text-teal-300/90">
            The shift
          </p>
          <h2 className="mt-3 text-center font-display text-3xl font-semibold text-white md:text-4xl">
            Move faster without sacrificing control
          </h2>
          <p className="mx-auto mt-4 max-w-2xl text-center text-lg text-slate-400">
            Teams adopt Cloud QAMS when spreadsheets, disconnected portals, and manual chasing stop
            scaling — especially as products, suppliers, and regulators demand clearer proof.
          </p>
        </FadeIn>

        <div className="mt-16 grid gap-6 md:grid-cols-3">
          {problems.map((p, i) => (
            <FadeIn key={p.title} delay={i * 0.08}>
              <article className="glass-panel group relative h-full overflow-hidden rounded-3xl p-8 transition hover:border-teal-400/30">
                <div className="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-transparent via-cyan-400/60 to-transparent opacity-0 transition group-hover:opacity-100" />
                <div className="mb-4 flex h-11 w-11 items-center justify-center rounded-2xl bg-teal-500/15 text-lg font-bold text-teal-300">
                  {i + 1}
                </div>
                <h3 className="font-display text-xl font-semibold text-white">{p.title}</h3>
                <p className="mt-3 text-sm leading-relaxed text-slate-400">{p.body}</p>
              </article>
            </FadeIn>
          ))}
        </div>
      </div>
    </section>
  )
}
