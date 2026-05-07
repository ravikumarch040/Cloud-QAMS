import { motion, useReducedMotion, useScroll, useSpring } from 'framer-motion'

export function ScrollProgress() {
  const reduce = useReducedMotion()
  const { scrollYProgress } = useScroll()
  const scaleX = useSpring(scrollYProgress, {
    stiffness: 120,
    damping: 28,
    restDelta: 0.001,
  })

  if (reduce) return null

  return (
    <motion.div
      className="pointer-events-none fixed left-0 right-0 top-0 z-[60] h-[3px] origin-left bg-gradient-to-r from-teal-400 via-cyan-400 to-sky-400"
      style={{ scaleX }}
      aria-hidden
    />
  )
}
