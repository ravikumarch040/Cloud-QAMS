import { site, disclaimer } from '../content'

export function Footer() {
  const year = new Date().getFullYear()

  return (
    <footer className="border-t border-white/10 bg-[#020617] px-4 py-12 md:px-8">
      <div className="mx-auto flex max-w-6xl flex-col gap-8 md:flex-row md:items-start md:justify-between">
        <div>
          <div className="flex items-center gap-2">
            <span className="flex h-9 w-9 items-center justify-center rounded-xl bg-gradient-to-br from-cyan-400 to-teal-500 text-sm font-bold text-slate-950">
              Q
            </span>
            <span className="font-display text-lg font-semibold text-white">{site.name}</span>
          </div>
          <p className="mt-4 max-w-md text-sm leading-relaxed text-slate-500">
            {disclaimer}
          </p>
        </div>
        <div className="flex flex-col gap-4 text-sm text-slate-400 md:items-end">
          <a
            href={site.contactMailto}
            className="hover:text-cyan-300 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-cyan-400"
          >
            Contact
          </a>
          <a href="#top" className="hover:text-cyan-300">
            Back to top
          </a>
          <p className="text-xs text-slate-600">© {year} {site.name}. All rights reserved.</p>
        </div>
      </div>
    </footer>
  )
}
