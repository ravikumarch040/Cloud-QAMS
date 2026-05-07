import {
  motion,
  useMotionValueEvent,
  useReducedMotion,
  useScroll,
} from 'framer-motion'
import { useRef, useState } from 'react'
import { architectureScenes } from '../content'
import { withBase } from '../baseUrl'

function layerStrength(scene: number, focus: number) {
  const d = Math.abs(scene - focus)
  if (d === 0) return 1
  if (d === 1) return 0.42
  return 0.18
}

export function ArchitectureStory() {
  const reduce = useReducedMotion()
  const containerRef = useRef<HTMLElement>(null)
  const { scrollYProgress } = useScroll({
    target: containerRef,
    offset: ['start start', 'end end'],
  })
  const [scene, setScene] = useState(0)

  useMotionValueEvent(scrollYProgress, 'change', (v) => {
    const idx = Math.min(
      architectureScenes.length - 1,
      Math.floor(v * architectureScenes.length),
    )
    setScene(idx)
  })

  const caption = architectureScenes[scene]

  const oUsers = layerStrength(scene, 0)
  const oGate = layerStrength(scene, 1)
  const oWork = layerStrength(scene, 2)
  const oFlow = layerStrength(scene, 3)
  const oCore = layerStrength(scene, 4)

  return (
    <section id="architecture" ref={containerRef} className="relative bg-[#030914]">
      <div className="relative min-h-[460vh] px-4 md:px-8">
        <div className="sticky top-0 flex min-h-[100svh] flex-col justify-center pt-24 pb-12">
          <div className="mx-auto w-full max-w-6xl">
            <p className="text-xs font-bold uppercase tracking-[0.35em] text-teal-300/90">
              Platform flow
            </p>
            <h2 className="mt-3 font-display text-3xl font-semibold text-white md:text-4xl">
              Follow the journey from people to proof
            </h2>
            <p className="mt-4 max-w-2xl text-lg text-slate-400">
              Scroll this chapter to see — at a glance — how experiences, automation, and records fit
              together. This is a conceptual map for stakeholders, not an integration catalog.
            </p>

            <div className="mt-10 grid gap-8 lg:grid-cols-[minmax(0,1fr)_340px] lg:items-start">
              <motion.div
                layout
                className="glass-panel relative overflow-hidden rounded-3xl border border-cyan-400/15 p-6 shadow-2xl shadow-cyan-500/10 md:p-8"
              >
                <div className="pointer-events-none absolute -left-24 top-10 h-52 w-52 rounded-full bg-cyan-500/10 blur-3xl" />
                <div className="pointer-events-none absolute -right-20 bottom-0 h-48 w-48 rounded-full bg-teal-500/10 blur-3xl" />

                <svg
                  viewBox="0 0 900 520"
                  className="relative z-[1] h-auto w-full"
                  role="img"
                  aria-label="Animated conceptual diagram of Cloud QAMS platform flow"
                >
                  <defs>
                    <linearGradient id="g-line" x1="0%" y1="0%" x2="100%" y2="0%">
                      <stop offset="0%" stopColor="#5eead6" stopOpacity="0.2" />
                      <stop offset="50%" stopColor="#22d3ee" stopOpacity="0.95" />
                      <stop offset="100%" stopColor="#14b8a6" stopOpacity="0.35" />
                    </linearGradient>
                    <filter id="glow" x="-20%" y="-20%" width="140%" height="140%">
                      <feGaussianBlur stdDeviation="6" result="blur" />
                      <feMerge>
                        <feMergeNode in="blur" />
                        <feMergeNode in="SourceGraphic" />
                      </feMerge>
                    </filter>
                  </defs>

                  {/* Connector backbone */}
                  <motion.path
                    d="M 450 92 L 450 162 L 450 248 L 450 328 L 450 410"
                    fill="none"
                    stroke="url(#g-line)"
                    strokeWidth="4"
                    strokeLinecap="round"
                    animate={{ opacity: 0.35 + oFlow * 0.55 }}
                    transition={{ duration: 0.35 }}
                    className={reduce || scene < 3 ? '' : 'flow-dash'}
                  />

                  {/* Scene A — People */}
                  <motion.g animate={{ opacity: oUsers }} transition={{ duration: 0.35 }}>
                    {[
                      { cx: 210, label: 'QA' },
                      { cx: 330, label: 'Ops' },
                      { cx: 450, label: 'Leaders' },
                      { cx: 570, label: 'Audit' },
                      { cx: 690, label: 'Partners' },
                    ].map((u) => (
                      <g key={u.label}>
                        <circle
                          cx={u.cx}
                          cy={72}
                          r={26}
                          fill="rgb(15 23 42)"
                          stroke="rgb(94 234 212)"
                          strokeWidth="2"
                          filter="url(#glow)"
                        />
                        <text
                          x={u.cx}
                          y={76}
                          textAnchor="middle"
                          fill="rgb(204 251 241)"
                          fontSize="14"
                          fontFamily="Instrument Sans, system-ui"
                          fontWeight="600"
                        >
                          {u.label}
                        </text>
                        <line
                          x1={u.cx}
                          y1={98}
                          x2={450}
                          y2={138}
                          stroke="rgb(45 212 191)"
                          strokeOpacity={0.35}
                          strokeWidth="2"
                        />
                      </g>
                    ))}
                  </motion.g>

                  {/* Scene B — Protected edge */}
                  <motion.g animate={{ opacity: oGate }} transition={{ duration: 0.35 }}>
                    <rect
                      x={318}
                      y={138}
                      width={264}
                      height={72}
                      rx={20}
                      fill="rgb(8 47 73 / 0.55)"
                      stroke="rgb(34 211 238)"
                      strokeWidth="2"
                    />
                    <text
                      x={450}
                      y={172}
                      textAnchor="middle"
                      fill="rgb(207 250 254)"
                      fontSize="16"
                      fontFamily="Instrument Sans, system-ui"
                      fontWeight="600"
                    >
                      Protected access & routing
                    </text>
                    <text
                      x={450}
                      y={194}
                      textAnchor="middle"
                      fill="rgb(148 163 184)"
                      fontSize="12"
                      fontFamily="DM Sans, system-ui"
                    >
                      Identity-aware edge · policy enforced
                    </text>
                  </motion.g>

                  {/* Scene C — Workspaces */}
                  <motion.g animate={{ opacity: oWork }} transition={{ duration: 0.35 }}>
                    {[
                      { x: 168, label: 'Operations' },
                      { x: 288, label: 'Auditor' },
                      { x: 408, label: 'Supplier' },
                      { x: 528, label: 'Admin' },
                      { x: 648, label: 'Executive' },
                    ].map((w) => (
                      <g key={w.label}>
                        <rect
                          x={w.x}
                          y={248}
                          width={112}
                          height={56}
                          rx={14}
                          fill="rgb(15 23 42 / 0.9)"
                          stroke="rgb(148 163 184 / 0.35)"
                          strokeWidth="1.5"
                        />
                        <text
                          x={w.x + 56}
                          y={282}
                          textAnchor="middle"
                          fill="rgb(226 232 240)"
                          fontSize="13"
                          fontFamily="DM Sans, system-ui"
                          fontWeight="600"
                        >
                          {w.label}
                        </text>
                      </g>
                    ))}
                  </motion.g>

                  {/* Scene D — Pulses into core */}
                  <motion.g animate={{ opacity: oFlow }} transition={{ duration: 0.35 }}>
                    <path
                      d="M 224 304 Q 450 360 450 328"
                      fill="none"
                      stroke="rgb(45 212 191)"
                      strokeWidth="2"
                      strokeOpacity="0.45"
                      className={reduce ? '' : 'flow-dash'}
                    />
                    <path
                      d="M 676 304 Q 450 360 450 328"
                      fill="none"
                      stroke="rgb(45 212 191)"
                      strokeWidth="2"
                      strokeOpacity="0.45"
                      className={reduce ? '' : 'flow-dash'}
                    />
                  </motion.g>

                  {/* Scene E — Core + records */}
                  <motion.g animate={{ opacity: oCore }} transition={{ duration: 0.35 }}>
                    <rect
                      x={318}
                      y={328}
                      width={264}
                      height={88}
                      rx={22}
                      fill="rgb(6 78 59 / 0.35)"
                      stroke="rgb(45 212 191)"
                      strokeWidth="2"
                    />
                    <text
                      x={450}
                      y={362}
                      textAnchor="middle"
                      fill="rgb(204 251 241)"
                      fontSize="17"
                      fontFamily="Instrument Sans, system-ui"
                      fontWeight="700"
                    >
                      Regulated platform core
                    </text>
                    <text
                      x={450}
                      y={386}
                      textAnchor="middle"
                      fill="rgb(148 163 184)"
                      fontSize="12"
                      fontFamily="DM Sans, system-ui"
                    >
                      Workflows · signatures · audit narrative · automation
                    </text>

                    <rect
                      x={248}
                      y={410}
                      width={404}
                      height={88}
                      rx={22}
                      fill="rgb(2 6 23 / 0.65)"
                      stroke="rgb(56 189 248 / 0.35)"
                      strokeWidth="1.5"
                    />
                    <text
                      x={450}
                      y={448}
                      textAnchor="middle"
                      fill="rgb(224 242 254)"
                      fontSize="15"
                      fontFamily="Instrument Sans, system-ui"
                      fontWeight="600"
                    >
                      Trusted records, search & governed AI assist
                    </text>
                    <text
                      x={450}
                      y={472}
                      textAnchor="middle"
                      fill="rgb(148 163 184)"
                      fontSize="12"
                      fontFamily="DM Sans, system-ui"
                    >
                      Human approval on regulated actions · citations preserved
                    </text>
                  </motion.g>
                </svg>
              </motion.div>

              <aside className="lg:sticky lg:top-28">
                <motion.div
                  key={caption.title}
                  initial={reduce ? false : { opacity: 0, x: 16 }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ duration: 0.35 }}
                  className="rounded-3xl border border-white/10 bg-slate-950/70 p-7 backdrop-blur-md"
                >
                  <p className="text-[11px] font-bold uppercase tracking-[0.35em] text-cyan-300/90">
                    Scene {scene + 1} / {architectureScenes.length}
                  </p>
                  <h3 className="mt-3 font-display text-2xl font-semibold text-white">
                    {caption.title}
                  </h3>
                  <p className="mt-4 text-sm leading-relaxed text-slate-400">{caption.subtitle}</p>
                  <div className="mt-6 flex flex-wrap gap-2">
                    {architectureScenes.map((_, i) => (
                      <span
                        key={i}
                        className={`h-2 w-8 rounded-full transition ${
                          i === scene ? 'bg-gradient-to-r from-teal-400 to-cyan-400' : 'bg-white/10'
                        }`}
                      />
                    ))}
                  </div>
                </motion.div>

                <div className="mt-8 overflow-hidden rounded-3xl border border-white/10 ring-1 ring-cyan-400/10">
                  <video
                    className="h-48 w-full object-cover opacity-90"
                    controls
                    muted
                    playsInline
                    poster="https://images.unsplash.com/photo-1587854692152-cbe660dbde88?auto=format&fit=crop&w=900&q=75"
                  >
                    <source
                      src="https://assets.mixkit.co/videos/preview/mixkit-woman-working-in-a-laboratory-4862-large.mp4"
                      type="video/mp4"
                    />
                  </video>
                  <p className="border-t border-white/10 bg-slate-950/80 px-4 py-3 text-xs text-slate-400">
                    Story-level demo reel placeholder — swap for your product walkthrough when ready.
                  </p>
                </div>
              </aside>
            </div>
          </div>
        </div>
      </div>

      <div className="mx-auto max-w-6xl px-4 pb-24 md:px-8">
        <div className="rounded-3xl border border-white/10 bg-slate-950/40 p-6 md:p-10">
          <h3 className="font-display text-xl font-semibold text-white md:text-2xl">
            Engineering blueprint snapshot
          </h3>
          <p className="mt-3 max-w-3xl text-sm text-slate-400">
            For stakeholders who want the canonical technical diagram, here is the current high-level
            export from the repository wiki — useful alongside the animated overview above.
          </p>
          <img
            src={withBase('architecture-overview.png')}
            alt="High-level Cloud QAMS architecture diagram"
            width={1600}
            height={900}
            loading="lazy"
            decoding="async"
            className="mt-8 w-full rounded-2xl border border-white/10 shadow-xl shadow-black/40"
          />
        </div>
      </div>
    </section>
  )
}
