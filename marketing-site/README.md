# Cloud QAMS marketing site

Single-page React + Vite experience for GitHub Pages: product story, visuals, scroll-driven architecture flow, and contact CTAs.

## Prerequisites

- Node.js 20+ (CI uses 22)
- npm

## Local development

```powershell
cd marketing-site
npm install
npm run dev
```

Open the URL Vite prints (default `http://localhost:5173/`). Development uses `base: '/'` so assets resolve without the GitHub Pages prefix.

## Production build

```powershell
cd marketing-site
npm run build
npm run preview
```

Preview serves with `base: '/Cloud-QAMS/'` only when `NODE_ENV`/`mode` is production — use `npm run preview` after a production build to spot-check asset paths.

## GitHub Pages (free)

1. In the GitHub repo, enable **Pages** with source **GitHub Actions**.
2. Confirm the repository name matches the Vite base path in [`vite.config.ts`](vite.config.ts) (`pagesBase`, currently `/Cloud-QAMS/`). If your repo slug differs, update `pagesBase` to `/<your-repo-name>/`.
3. Push to `main` under `marketing-site/` — workflow [`.github/workflows/deploy-pages.yml`](../.github/workflows/deploy-pages.yml) builds and deploys `marketing-site/dist`.
4. The workflow sets `VITE_PUBLIC_SITE_URL` to `https://<owner>.github.io/<repo>` so Open Graph / Twitter image URLs resolve to `.../architecture-overview.png`.

### Contact & branding

Edit [`src/content.ts`](src/content.ts): `contactMailto`, `scheduleUrl`, and product naming.

### Hero / demo media

Hero video uses a Mixkit preview URL by default. Replace with your own files under `public/` for full control (keep files reasonably small for GitHub Pages).

## Disclaimer

Marketing copy includes an informational disclaimer in the footer and contact section — adjust with legal/compliance guidance before broad publication.
