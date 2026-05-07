import { Suspense, lazy } from 'react'
import { MotionConfig } from 'framer-motion'
import { Header } from './components/Header'
import { ScrollProgress } from './components/ScrollProgress'
import { Hero } from './components/Hero'
import { ProblemOutcome } from './components/ProblemOutcome'
import { CapabilityPillars } from './components/CapabilityPillars'
import { Differentiators } from './components/Differentiators'
import { PersonaStrip } from './components/PersonaStrip'
import { TrustSection } from './components/TrustSection'
import { ContactSection } from './components/ContactSection'
import { Footer } from './components/Footer'
import { site } from './content'

const ArchitectureStory = lazy(async () => {
  const mod = await import('./components/ArchitectureStory')
  return { default: mod.ArchitectureStory }
})

function ArchitectureFallback() {
  return (
    <section className="flex min-h-[70vh] items-center justify-center px-4 py-24 md:px-8">
      <div className="h-72 w-full max-w-5xl animate-pulse rounded-3xl bg-slate-900/60 ring-1 ring-white/10" />
    </section>
  )
}

export default function App() {
  return (
    <MotionConfig reducedMotion="user">
      <div className="font-body bg-[#050b14] text-slate-200 antialiased">
        <ScrollProgress />
        <Header />
        <main>
          <Hero />
          <ProblemOutcome />
          <CapabilityPillars />
          <Differentiators />
          <PersonaStrip />
          <Suspense fallback={<ArchitectureFallback />}>
            <ArchitectureStory />
          </Suspense>
          <TrustSection />
          <ContactSection />
        </main>
        <Footer />
        {/* Hidden SEO helpers */}
        <span className="sr-only">{site.tagline}</span>
      </div>
    </MotionConfig>
  )
}
