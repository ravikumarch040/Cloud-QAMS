import { motion, useReducedMotion } from 'framer-motion'
import { disclaimer, site } from '../content'

export function ContactSection() {
  const reduce = useReducedMotion()

  return (
    <section id="contact" className="relative px-4 py-24 md:px-8">
      <div className="mx-auto max-w-6xl overflow-hidden rounded-[2rem] border border-cyan-400/20 bg-gradient-to-br from-slate-900 via-slate-950 to-[#041624] p-[1px] shadow-2xl shadow-cyan-500/15">
        <div className="relative rounded-[calc(2rem-1px)] px-6 py-14 md:px-16 md:py-16">
          <div className="pointer-events-none absolute -left-32 top-0 h-72 w-72 rounded-full bg-teal-500/15 blur-3xl" />
          <div className="pointer-events-none absolute -right-24 bottom-0 h-64 w-64 rounded-full bg-cyan-400/15 blur-3xl" />

          <div className="relative grid gap-12 lg:grid-cols-[1.1fr_0.9fr] lg:items-center">
            <div>
              <p className="text-xs font-bold uppercase tracking-[0.35em] text-teal-300/90">
                Next step
              </p>
              <h2 className="mt-3 font-display text-3xl font-semibold text-white md:text-4xl">
                Ready to modernize your quality narrative?
              </h2>
              <p className="mt-5 max-w-xl text-lg text-slate-400">
                Invite us to a guided conversation — we will tailor the storyline to your modalities,
                franchise footprint, and validation expectations.
              </p>
              <div className="mt-10 flex flex-wrap gap-4">
                <motion.a
                  href={site.contactMailto}
                  whileHover={reduce ? undefined : { y: -2 }}
                  whileTap={reduce ? undefined : { scale: 0.98 }}
                  className="inline-flex items-center justify-center rounded-full bg-gradient-to-r from-teal-400 to-cyan-400 px-10 py-3.5 text-base font-semibold text-slate-950 shadow-xl shadow-cyan-500/35 transition hover:brightness-110 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-cyan-300"
                >
                  Email the team
                </motion.a>
                <a
                  href={site.scheduleUrl}
                  className="inline-flex items-center justify-center rounded-full border border-white/15 bg-white/5 px-10 py-3.5 text-base font-semibold text-white backdrop-blur-sm transition hover:border-cyan-400/40 hover:bg-white/10 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-cyan-400"
                >
                  Book a walkthrough
                </a>
              </div>
              <p className="mt-8 max-w-xl text-xs leading-relaxed text-slate-500">{disclaimer}</p>
            </div>

            <motion.div
              className="glass-panel relative rounded-3xl p-8"
              initial={reduce ? false : { opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.5 }}
            >
              <p className="text-sm font-semibold text-cyan-100">What to expect</p>
              <ul className="mt-4 space-y-4 text-sm text-slate-400">
                <li className="flex gap-3">
                  <span className="mt-1 h-2 w-2 shrink-0 rounded-full bg-teal-400" />
                  <span>
                    A concise tour mapped to your personas — from investigators to supplier managers.
                  </span>
                </li>
                <li className="flex gap-3">
                  <span className="mt-1 h-2 w-2 shrink-0 rounded-full bg-cyan-400" />
                  <span>
                    Discussion of deployment footprint on Azure and integration landmarks (ERP, LIMS, PLM,
                    Microsoft 365).
                  </span>
                </li>
                <li className="flex gap-3">
                  <span className="mt-1 h-2 w-2 shrink-0 rounded-full bg-sky-400" />
                  <span>
                    Transparent roadmap alignment — including AI governance and validation evidence themes.
                  </span>
                </li>
              </ul>
              <img
                src="https://images.unsplash.com/photo-1522071820081-009f0129c71c?auto=format&fit=crop&w=900&q=75"
                alt=""
                width={640}
                height={400}
                loading="lazy"
                decoding="async"
                className="mt-8 h-44 w-full rounded-2xl object-cover opacity-90 ring-1 ring-white/10"
              />
            </motion.div>
          </div>
        </div>
      </div>
    </section>
  )
}
