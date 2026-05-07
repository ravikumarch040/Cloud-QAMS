import { trustPoints } from '../content'
import { FadeIn } from './FadeIn'

export function TrustSection() {
  return (
    <section id="trust" className="relative px-4 py-24 md:px-8">
      <div className="mx-auto max-w-6xl">
        <FadeIn>
          <h2 className="font-display text-3xl font-semibold text-white md:text-4xl">
            Enterprise-grade foundations
          </h2>
          <p className="mt-4 max-w-2xl text-lg text-slate-400">
            Buyers should expect contemporary cloud security and operational transparency — described here
            in language executives understand; deeper assurance artifacts belong in your procurement process.
          </p>
        </FadeIn>

        <div className="mt-14 grid gap-6 md:grid-cols-2">
          {trustPoints.map((t, i) => (
            <FadeIn key={t.title} delay={i * 0.06}>
              <article className="glass-panel h-full rounded-3xl p-8">
                <div className="mb-3 h-1.5 w-16 rounded-full bg-gradient-to-r from-cyan-400 to-teal-400" />
                <h3 className="font-display text-xl font-semibold text-white">{t.title}</h3>
                <p className="mt-3 text-sm leading-relaxed text-slate-400">{t.body}</p>
              </article>
            </FadeIn>
          ))}
        </div>
      </div>
    </section>
  )
}
