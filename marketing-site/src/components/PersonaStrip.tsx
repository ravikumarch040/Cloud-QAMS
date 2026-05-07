import { personas } from '../content'
import { FadeIn } from './FadeIn'

export function PersonaStrip() {
  return (
    <section id="personas" className="relative px-4 py-20 md:px-8">
      <div className="mx-auto max-w-6xl">
        <FadeIn>
          <h2 className="font-display text-3xl font-semibold text-white md:text-4xl">
            Experiences tuned to every stakeholder
          </h2>
          <p className="mt-4 max-w-2xl text-lg text-slate-400">
            Each persona interacts with the same trusted platform — through lenses that emphasize what
            matters for their job.
          </p>
        </FadeIn>

        <div className="mt-12 flex snap-x snap-mandatory gap-4 overflow-x-auto pb-4 md:grid md:grid-cols-5 md:overflow-visible">
          {personas.map((p, i) => (
            <FadeIn
              key={p.role}
              className="min-w-[240px] shrink-0 snap-start md:min-w-0"
              delay={i * 0.06}
            >
              <article className="glass-panel flex h-full flex-col rounded-3xl p-6">
                <div className="text-xs font-bold uppercase tracking-[0.25em] text-teal-300/90">
                  Persona
                </div>
                <h3 className="mt-2 font-display text-lg font-semibold text-white">{p.role}</h3>
                <p className="mt-3 flex-1 text-sm leading-relaxed text-slate-400">{p.gain}</p>
              </article>
            </FadeIn>
          ))}
        </div>

        <FadeIn className="mt-12">
          <img
            src="https://images.unsplash.com/photo-1556761175-5973dc0f32e7?auto=format&fit=crop&w=1400&q=75"
            alt="Team collaborating in office"
            width={1400}
            height={560}
            loading="lazy"
            decoding="async"
            className="h-56 w-full rounded-3xl object-cover ring-1 ring-white/10 md:h-72"
          />
        </FadeIn>
      </div>
    </section>
  )
}
