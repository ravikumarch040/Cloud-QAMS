import { motion, useMotionValueEvent, useReducedMotion, useScroll } from 'framer-motion'
import { useState } from 'react'
import { site } from '../content'

const links = [
  { href: '#pillars', label: 'Capabilities' },
  { href: '#compare', label: 'Why QAMS' },
  { href: '#architecture', label: 'Platform flow' },
  { href: '#contact', label: 'Contact' },
]

export function Header() {
  const reduce = useReducedMotion()
  const { scrollY } = useScroll()
  const [scrolled, setScrolled] = useState(false)

  useMotionValueEvent(scrollY, 'change', (y) => {
    if (!reduce) setScrolled(y > 36)
  })

  return (
    <header className="fixed inset-x-0 top-0 z-50 px-4 py-3 md:px-8">
      <div
        className={`mx-auto flex max-w-6xl items-center justify-between gap-4 rounded-2xl border px-3 py-2 transition-colors md:px-4 ${
          scrolled || reduce
            ? 'border-white/10 bg-slate-950/75 shadow-lg shadow-black/20 backdrop-blur-xl'
            : 'border-transparent bg-transparent'
        }`}
      >
        <a href="#top" className="flex items-center gap-2">
          <span className="flex h-9 w-9 items-center justify-center rounded-xl bg-gradient-to-br from-cyan-400 to-teal-500 text-sm font-bold text-slate-950 shadow-lg shadow-cyan-500/25">
            Q
          </span>
          <span className="font-display text-lg font-semibold tracking-tight text-white">
            {site.name}
          </span>
        </a>
        <nav
          className="hidden items-center gap-6 text-sm font-medium text-slate-300 md:flex"
          aria-label="Primary"
        >
          {links.map((l) => (
            <a
              key={l.href}
              href={l.href}
              className="transition-colors hover:text-cyan-300 focus-visible:rounded-md focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-cyan-400"
            >
              {l.label}
            </a>
          ))}
        </nav>
        <motion.a
          href={site.contactMailto}
          whileHover={reduce ? undefined : { scale: 1.02 }}
          whileTap={reduce ? undefined : { scale: 0.98 }}
          className="rounded-full bg-gradient-to-r from-teal-400 to-cyan-400 px-4 py-2 text-sm font-semibold text-slate-950 shadow-lg shadow-cyan-500/30 transition hover:brightness-110 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-cyan-300"
        >
          Talk to us
        </motion.a>
      </div>
    </header>
  )
}
