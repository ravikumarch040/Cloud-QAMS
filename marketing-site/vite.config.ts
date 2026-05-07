import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

/** GitHub Pages project-site path — must match repository name on GitHub */
const pagesBase = '/Cloud-QAMS/'

export default defineConfig(({ mode }) => {
  const base = mode === 'production' ? pagesBase : '/'

  return {
    base,
    plugins: [
      react(),
      tailwindcss(),
      {
        name: 'inject-public-meta',
        transformIndexHtml(html) {
          const site = process.env.VITE_PUBLIC_SITE_URL?.replace(/\/$/, '') ?? ''
          const canonical = site ? `${site}/` : ''
          const ogImage = site ? `${site}/architecture-overview.png` : ''

          let out = html
            .replaceAll('__CANONICAL__', canonical)
            .replaceAll('__OG_IMAGE__', ogImage)

          if (!canonical) {
            out = out.replace(/\s*<link rel="canonical" href="" \/>\s*\n?/, '\n')
          }
          if (!ogImage) {
            out = out.replace(/\s*<meta property="og:image" content="" \/>\s*\n?/, '\n')
            out = out.replace(/\s*<meta name="twitter:image" content="" \/>\s*\n?/, '\n')
          }
          return out
        },
      },
    ],
  }
})
