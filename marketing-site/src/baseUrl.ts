/** Vite base URL for GitHub Pages project sites */
export function withBase(path: string): string {
  const base = import.meta.env.BASE_URL.replace(/\/$/, '')
  const p = path.startsWith('/') ? path.slice(1) : path
  return `${base}/${p}`
}
