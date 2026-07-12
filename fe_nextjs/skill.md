---
name: frontend-design
description: Tạo giao diện trang web game giải trí văn phòng với NextJS — production-grade, 
             thị giác ấn tượng, tránh aesthetic AI generic.
stack: NextJS (App Router), Tailwind CSS, Framer Motion, TypeScript
---

## Mục tiêu thiết kế

Trang game giải trí văn phòng cần cân bằng giữa:
- **Vui vẻ, nhẹ nhàng** — đủ để giải stress sau giờ họp
- **Chuyên nghiệp vừa phải** — không loè loẹt, phù hợp môi trường office
- **Tốc độ** — load nhanh, transition mượt, không lag máy yếu công ty

---

## Design Thinking

Trước khi code, xác định rõ:

- **Tone**: Chọn 1 trong: Retro-pixel (8-bit nostalgia), Flat-playful (Duolingo-ish), 
  Neon-dark (cyberpunk nhẹ), Paper/Doodle (hand-drawn), Card-game (board game aesthetic).
  Cam kết 1 hướng, đừng pha tạp.
- **Màu sắc**: Dùng 1–2 màu dominant + 1 accent sắc nét. 
  Tránh: purple gradient trên nền trắng (cliché AI).
  Gợi ý tốt: Amber + Slate, Teal + Cream, Coral + Navy, Lime + Charcoal.
- **Typography**: Dùng font có cá tính. Gợi ý:
  - Display: "Press Start 2P" (pixel), "Fredoka One" (playful), "Space Mono" (retro-tech)
  - Body: "DM Sans", "Nunito", "Sora"
  - Tránh tuyệt đối: Arial, Inter, Roboto, system-ui
- **Layout**: Bất đối xứng có kiểm soát. Game cards grid với hover lift effect. 
  Hero section không dùng centered-text-on-gradient thông thường.

---

## Tech Stack (NextJS cụ thể)

### Cấu trúc thư mục đề nghị
app/
layout.tsx          ← font import (next/font/google), global CSS vars
page.tsx            ← Home / Game lobby
games/[slug]/
page.tsx          ← Game detail / play page
components/
ui/                 ← Button, Card, Badge, Modal
game/               ← GameCard, Leaderboard, Timer, ScoreBoard
layout/             ← Navbar, Sidebar, Footer
lib/
hooks/              ← useTimer, useScore, useSound
utils/              ← formatScore, shuffle, etc.
public/
sounds/             ← sfx (click, win, lose)
sprites/            ← game assets nếu có

### Font setup (next/font/google)
```tsx
// app/layout.tsx
import { Fredoka_One, DM_Sans } from 'next/font/google'

const display = Fredoka_One({ weight: '400', subsets: ['latin'], variable: '--font-display' })
const body = DM_Sans({ subsets: ['latin'], variable: '--font-body' })
```

### CSS Variables (globals.css)
```css
:root {
  --color-bg: #0f0f0f;
  --color-surface: #1a1a2e;
  --color-primary: #f59e0b;    /* amber — ví dụ */
  --color-accent: #10b981;     /* emerald */
  --color-text: #f1f5f9;
  --color-muted: #64748b;
  --font-display: var(--font-display);
  --font-body: var(--font-body);
  --radius: 12px;
  --shadow-game: 0 8px 32px rgba(245,158,11,0.15);
}
```

---

## Component Patterns

### GameCard (core component)
- Hover: scale(1.04) + box-shadow lift + subtle glow theo màu category
- Badge category góc trên phải
- Thumbnail 16:9, lazy loaded với next/image
- "Chơi ngay" button xuất hiện khi hover (Framer Motion AnimatePresence)

### Leaderboard
- Top 3 có crown icon + highlight đặc biệt
- Animation: staggered slide-in khi mount
- Avatar dùng DiceBear hoặc initials color-coded

### Timer / Score
- Số đếm animate (odometer effect hoặc flip card)
- Pulse khi gần hết giờ (CSS @keyframes pulse + color đổi sang đỏ)

### Lobby / Home
- Hero: Tên game lớn + tagline + CTA. KHÔNG dùng hero ảnh full màn hình mờ.
- Game grid: Masonry hoặc asymmetric columns, KHÔNG equal-height boring grid
- Category filter: Pill buttons với active state rõ ràng

---

## Animation Guidelines (Framer Motion)
```tsx
// Page transition
const pageVariants = {
  hidden: { opacity: 0, y: 20 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.4, ease: 'easeOut' } }
}

// Stagger children (game cards)
const containerVariants = {
  visible: { transition: { staggerChildren: 0.07 } }
}
```

- Page load: staggered reveal cho game cards (đừng fade tất cả cùng lúc)
- Win state: confetti (canvas-confetti) + score pop
- Micro-interaction: button press scale(0.96), hover scale(1.02)
- Tránh: animation quá nhiều trên cùng 1 màn hình — chọn 2–3 điểm nhấn

---

## Sound Design (tuỳ chọn nhưng khuyến khích)
```tsx
// lib/hooks/useSound.ts
const useSound = (src: string) => {
  const play = () => new Audio(src).play()
  return { play }
}
// Dùng: click, correct-answer, wrong-answer, countdown-tick, win-fanfare
```

---

## Accessibility & Performance

- Tất cả interactive element có focus-visible rõ ràng
- Game không autoplay sound — cần user gesture trước
- next/image cho mọi ảnh, blur placeholder
- `prefers-reduced-motion` media query — tắt animation nếu user cần
- Keyboard navigable cho game lobby

---

## Những gì KHÔNG làm

❌ Purple gradient trên nền trắng  
❌ Font Inter/Roboto/Arial  
❌ Equal-height boring card grid  
❌ Loading spinner thông thường — thay bằng skeleton phù hợp aesthetic  
❌ Modal full-screen blur background mà không có animation  
❌ "Flat design" không có depth nào — phải có shadow/layer dù minimal  

---

## Checklist trước khi ship

- [ ] Dark/Light mode (nếu cần) dùng CSS variables + next-themes
- [ ] Mobile responsive: game cards stack đẹp trên 375px
- [ ] Favicon và OG image phù hợp theme
- [ ] Error state và empty state được thiết kế (không để mặc định)
- [ ] Score/Leaderboard persist (localStorage hoặc API)