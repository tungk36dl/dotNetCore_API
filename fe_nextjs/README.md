# Frontend — ProjectCore

UI quản lý user / role cho [ProjectCore API](https://github.com/tungk36dl/dotNetCore_API). Stack: **Next.js 16** (App Router), React 19, TypeScript, Tailwind CSS 4, Zustand, Axios.

Hướng dẫn clone toàn bộ repo, Docker và API: [README gốc](../README.md).

## Chạy local

```bash
cp .env.local.example .env.local
npm install
npm run dev
```

Mở [http://localhost:3000](http://localhost:3000).

`NEXT_PUBLIC_API_URL` mặc định: `http://localhost:5036` (khớp profile `http` của API).

## Scripts

| Lệnh | Mô tả |
|------|--------|
| `npm run dev` | Dev server |
| `npm run build` | Production build |
| `npm run start` | Chạy bản build |
| `npm run lint` | ESLint |

## Cấu trúc chính

```text
app/(auth)/login          # Đăng nhập (public)
app/(dashboard)/          # Users, roles (cần JWT)
components/               # UI + layout
services/                 # Gọi API
stores/                   # Zustand
lib/api.ts                # Axios + refresh token
```
