/**
 * Shared Framer Motion variants — import from here to stay consistent
 * across all pages.
 */

/** Fade + slide up — use for page sections and containers */
export const fadeUp = {
  hidden:  { opacity: 0, y: 14 },
  visible: {
    opacity: 1, y: 0,
    transition: { duration: 0.35, ease: [0.22, 1, 0.36, 1] },
  },
};

/** Stagger children container */
export const staggerContainer = {
  hidden:  {},
  visible: { transition: { staggerChildren: 0.06, delayChildren: 0.05 } },
};

/** Stagger item — fade + slight x slide (for table rows / list items) */
export const staggerItem = {
  hidden:  { opacity: 0, x: -8 },
  visible: {
    opacity: 1, x: 0,
    transition: { duration: 0.3, ease: [0.22, 1, 0.36, 1] },
  },
};

/** Form field reveal — stagger vertically */
export const fieldReveal = {
  hidden:  { opacity: 0, y: 10 },
  visible: {
    opacity: 1, y: 0,
    transition: { duration: 0.28, ease: "easeOut" },
  },
};

/** Card hover lift */
export const cardHover = {
  rest:  { y: 0, boxShadow: "0 0 0 0 transparent" },
  hover: {
    y: -2,
    boxShadow: "0 8px 32px rgba(200,155,79,0.12)",
    transition: { duration: 0.2 },
  },
};
