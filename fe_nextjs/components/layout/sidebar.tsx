"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Users, Shield, LayoutDashboard, LogOut } from "lucide-react";
import { useAuthStore } from "@/stores/auth-store";
import { authService } from "@/services/auth.service";
import { useRouter } from "next/navigation";

const navItems = [
  { href: "/",       label: "Dashboard", icon: LayoutDashboard },
  { href: "/users",  label: "Users",     icon: Users },
  { href: "/roles",  label: "Roles",     icon: Shield },
];

export function Sidebar() {
  const pathname = usePathname();
  const router   = useRouter();
  const { user, refreshToken, logout } = useAuthStore();

  const handleLogout = async () => {
    try {
      await authService.logout(refreshToken);
    } catch {
      // ignore
    }
    logout();
    document.cookie = "auth-token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
    router.push("/login");
  };

  return (
    <aside
      className="w-56 flex flex-col min-h-screen bg-void border-r border-rim"
      style={{ borderRightWidth: "1px" }}
    >
      {/* Brand */}
      <div className="px-6 pt-8 pb-6 border-b border-rim">
        <div className="flex items-center gap-2.5 mb-1">
          {/* Gold monogram */}
          <div
            className="w-7 h-7 bg-gold-ghost border border-gold-dim flex items-center justify-center shrink-0"
          >
            <span
              className="text-gold text-xs font-bold"
              style={{ fontFamily: "var(--font-jetbrains)", lineHeight: 1 }}
            >
              PC
            </span>
          </div>
          <span
            className="text-chalk text-lg leading-none"
            style={{ fontFamily: "var(--font-cormorant)", fontWeight: 600 }}
          >
            ProjectCore
          </span>
        </div>
        <p
          className="text-[10px] uppercase tracking-[0.2em] text-dim mt-1 pl-9"
        >
          Admin Console
        </p>
      </div>

      {/* Navigation */}
      <nav className="flex-1 py-4 px-3">
        <p className="text-[9px] uppercase tracking-[0.2em] text-ash px-3 mb-3 mt-2">
          Navigation
        </p>
        <ul className="space-y-0.5">
          {navItems.map((item) => {
            const isActive =
              item.href === "/"
                ? pathname === "/"
                : pathname.startsWith(item.href);
            return (
              <li key={item.href}>
                <Link
                  href={item.href}
                  className={`
                    flex items-center gap-3 px-3 py-2.5 transition-all duration-150 group relative
                    ${isActive
                      ? "text-chalk bg-surface-hi"
                      : "text-dim hover:text-mist hover:bg-surface"
                    }
                  `}
                >
                  {/* Active indicator */}
                  {isActive && (
                    <span className="absolute left-0 top-1/2 -translate-y-1/2 w-0.5 h-5 bg-gold" />
                  )}
                  <item.icon
                    className={`w-4 h-4 shrink-0 transition-colors ${
                      isActive ? "text-gold" : "text-ash group-hover:text-dim"
                    }`}
                  />
                  <span
                    className="text-xs font-medium tracking-wide"
                  >
                    {item.label}
                  </span>
                </Link>
              </li>
            );
          })}
        </ul>
      </nav>

      {/* Profile + Logout */}
      <div className="border-t border-rim px-3 py-4">
        <div className="flex items-center gap-2.5 px-3 py-2 mb-1">
          <div
            className="w-7 h-7 flex items-center justify-center shrink-0 border border-gold-dim bg-gold-ghost"
          >
            <span
              className="text-gold text-xs font-semibold"
              style={{ fontFamily: "var(--font-jetbrains)" }}
            >
              {user?.userName?.[0]?.toUpperCase() ?? "?"}
            </span>
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-xs font-medium text-chalk truncate">
              {user?.userName ?? "—"}
            </p>
            <p className="text-[10px] text-dim truncate mt-0.5">
              {user?.email ?? "—"}
            </p>
          </div>
        </div>
        <button
          onClick={handleLogout}
          className="flex items-center gap-3 px-3 py-2 w-full text-dim hover:text-chalk hover:bg-surface transition-colors duration-150 group"
        >
          <LogOut className="w-4 h-4 text-ash group-hover:text-dim" />
          <span className="text-xs tracking-wide">Sign out</span>
        </button>
      </div>
    </aside>
  );
}
