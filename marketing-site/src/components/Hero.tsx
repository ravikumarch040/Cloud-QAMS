import { motion, useReducedMotion } from 'framer-motion'
import { hero, site } from '../content'

const labVideoMp4 =
  'https://assets.mixkit.co/videos/preview/mixkit-woman-working-in-a-laboratory-4862-large.mp4'

export function Hero() {
  const reduce = useReducedMotion()

  return (
    <section
      id="top"
      className="relative flex min-h-[100svh] flex-col justify-center overflow-hidden pt-28 pb-16"
    >
      <div className="mesh-bg absolute inset-0" aria-hidden />
      <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_center,_transparent_0%,_#050b14_75%)]" />

      {!reduce && (
        <video
          className="absolute inset-0 h-full w-full object-cover opacity-[0.22]"
          autoPlay
          muted
          loop
          playsInline
          poster="https://images.unsplash.com/photo-1579684385127-1ef15d508138?auto=format&fit=crop&w=1600&q=70"
          aria-hidden
        >
          <source src={labVideoMp4} type="video/mp4" />
        </video>
      )}

      <div className="relative mx-auto grid max-w-6xl gap-12 px-4 md:grid-cols-[1.15fr_0.85fr] md:px-8 md:gap-16">
        <div>
          <motion.p
            className="mb-4 inline-flex items-center gap-2 rounded-full border border-cyan-400/25 bg-cyan-400/10 px-4 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-cyan-200"
            initial={reduce ? false : { opacity: 0, y: 16 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
          >
            Azure-native SaaS
          </motion.p>
          <motion.h1
            className="font-display text-4xl font-semibold leading-[1.08] tracking-tight text-white md:text-6xl"
            initial={reduce ? false : { opacity: 0, y: 24 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6, delay: 0.05 }}
          >
            {hero.headline}
          </motion.h1>
          <motion.p
            className="mt-6 max-w-xl text-lg leading-relaxed text-slate-300 md:text-xl"
            initial={reduce ? false : { opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.55, delay: 0.12 }}
          >
            {hero.sub}
          </motion.p>
          <motion.div
            className="mt-10 flex flex-wrap gap-4"
            initial={reduce ? false : { opacity: 0, y: 16 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.2 }}
          >
            <a
              href={site.contactMailto}
              className="inline-flex items-center justify-center rounded-full bg-gradient-to-r from-teal-400 to-cyan-400 px-8 py-3 text-base font-semibold text-slate-950 shadow-xl shadow-cyan-500/35 transition hover:brightness-110 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-cyan-300"
            >
              {hero.primaryCta}
            </a>
            <a
              href="#architecture"
              className="inline-flex items-center justify-center rounded-full border border-white/15 bg-white/5 px-8 py-3 text-base font-semibold text-white backdrop-blur-sm transition hover:border-cyan-400/40 hover:bg-white/10 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-cyan-400"
            >
              {hero.secondaryCta}
            </a>
          </motion.div>
        </div>

        <motion.div
          className="relative"
          initial={reduce ? false : { opacity: 0, scale: 0.96 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.65, delay: 0.15 }}
        >
          <div className="glass-panel relative overflow-hidden rounded-3xl p-6 shadow-2xl shadow-cyan-500/10 ring-1 ring-cyan-400/20">
            <div className="absolute -right-16 -top-16 h-56 w-56 rounded-full bg-cyan-500/20 blur-3xl" />
            <div className="absolute -bottom-24 -left-10 h-52 w-52 rounded-full bg-teal-500/15 blur-3xl" />

            <div className="relative space-y-4">
              <div className="flex items-center justify-between text-xs font-medium uppercase tracking-wider text-slate-400">
                <span>Quality command</span>
                <span className="rounded-full bg-emerald-500/15 px-2 py-0.5 text-emerald-300">
                  Live posture
                </span>
              </div>
              <div className="grid gap-3">
                {[
                  { label: 'Open CAPAs', value: 'Guided', tone: 'text-cyan-200' },
                  { label: 'Training due', value: 'Automated nudges', tone: 'text-teal-200' },
                  { label: 'Audit window', value: 'Evidence ready', tone: 'text-sky-200' },
                ].map((row) => (
                  <div
                    key={row.label}
                    className="flex items-center justify-between rounded-2xl border border-white/10 bg-slate-950/40 px-4 py-3"
                  >
                    <span className="text-sm text-slate-400">{row.label}</span>
                    <span className={`text-sm font-semibold ${row.tone}`}>{row.value}</span>
                  </div>
                ))}
              </div>
              <div className="rounded-2xl border border-dashed border-cyan-400/30 bg-gradient-to-br from-cyan-500/10 to-transparent p-4">
                <p className="text-sm font-medium text-cyan-100">Closed-loop traceability</p>
                <p className="mt-1 text-xs leading-relaxed text-slate-400">
                  From detection → containment → CAPA → training & documents → management review,
                  without losing the thread.
                </p>
              </div>
              <img
                src="https://images.unsplash.com/photo-1587854692152-cbe660dbde88?auto=format&fit=crop&w=900&q=75"
                alt="Laboratory bench with glassware and pipettes during research or quality testing"
                width={640}
                height={360}
                decoding="async"
                loading="eager"
                className="mt-2 h-40 w-full rounded-xl object-cover opacity-90 ring-1 ring-white/10"
              />
            </div>
          </div>
        </motion.div>
      </div>

      <div className="pointer-events-none absolute bottom-8 left-1/2 hidden -translate-x-1/2 md:block">
        {!reduce && (
          <motion.div
            className="flex flex-col items-center gap-2 text-xs text-slate-500"
            animate={{ y: [0, 6, 0] }}
            transition={{ repeat: Infinity, duration: 2.4, ease: 'easeInOut' }}
          >
            <span className="uppercase tracking-[0.3em]">Scroll</span>
            <span className="h-10 w-px bg-gradient-to-b from-cyan-400/60 to-transparent" />
          </motion.div>
        )}
      </div>
    </section>
  )
}
