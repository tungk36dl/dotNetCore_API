"use client";

import { Users, Shield, Activity, ArrowRight } from "lucide-react";
import { useAuthStore } from "@/stores/auth-store";
import { Header } from "@/components/layout/header";
import Link from "next/link";

const cards = [
  {
    title: "User Management",
    description: "Create accounts, assign roles, manage user profiles and access permissions.",
    icon: Users,
    href: "/users",
    tag: "USERS",
  },
  {
    title: "Role Management",
    description: "Define roles, configure permission sets, and control access boundaries.",
    icon: Shield,
    href: "/roles",
    tag: "ROLES",
  },
  {
    title: "Activity Logs",
    description: "Inspect system events, audit trails, and access history.",
    icon: Activity,
    href: "#",
    tag: "LOGS",
    disabled: true,
  },
];

export default function DashboardPage() {
  const { user } = useAuthStore();

  const hour = new Date().getHours();
  const greeting =
    hour < 12 ? "Good morning" : hour < 17 ? "Good afternoon" : "Good evening";

  return (
    <div className="animate-fade-in">
      <Header
        subtitle="Overview"
        title={`${greeting}, ${user?.userName ?? "Admin"}`}
      />

      {/* Quick stat strip */}
      <div className="flex items-center gap-6 mb-10 pb-6 border-b border-rim">
        {[
          { label: "System Status", value: "Online", ok: true },
          { label: "Session", value: user?.userName ?? "—" },
          { label: "Access Level", value: "Administrator" },
        ].map((s) => (
          <div key={s.label} className="flex items-center gap-3">
            <div>
              <p className="text-[10px] uppercase tracking-[0.15em] text-dim mb-0.5">
                {s.label}
              </p>
              <p
                className={`text-sm font-medium ${s.ok ? "text-ok" : "text-chalk"}`}
                style={{ fontFamily: "var(--font-jetbrains)" }}
              >
                {s.value}
              </p>
            </div>
            <div className="w-px h-7 bg-rim ml-3" />
          </div>
        ))}
      </div>

      {/* Module cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {cards.map((card) => (
          <Link
            key={card.title}
            href={card.href}
            className={`
              group block bg-surface border border-rim
              p-6 transition-all duration-200 relative overflow-hidden
              ${card.disabled
                ? "opacity-40 cursor-not-allowed pointer-events-none"
                : "hover:border-gold-dim hover:bg-surface-hi"
              }
            `}
          >
            {/* Top gold accent line on hover */}
            <div className="absolute top-0 left-0 right-0 h-px bg-gold scale-x-0 group-hover:scale-x-100 transition-transform duration-300 origin-left" />

            <div className="flex items-start justify-between mb-5">
              <div className="w-9 h-9 border border-rim group-hover:border-gold-dim bg-void flex items-center justify-center transition-colors duration-200">
                <card.icon className="w-4 h-4 text-dim group-hover:text-gold transition-colors duration-200" />
              </div>
              <span
                className="text-[9px] uppercase tracking-[0.2em] text-ash group-hover:text-gold-dim transition-colors duration-200"
                style={{ fontFamily: "var(--font-jetbrains)" }}
              >
                {card.tag}
              </span>
            </div>

            <h2
              className="text-xl text-chalk mb-2 leading-snug"
              style={{ fontFamily: "var(--font-cormorant)", fontWeight: 600 }}
            >
              {card.title}
            </h2>
            <p className="text-xs text-dim leading-relaxed">{card.description}</p>

            <div className="flex items-center gap-1 mt-5 text-[10px] uppercase tracking-widest text-ash group-hover:text-gold transition-colors duration-200">
              <span>Open</span>
              <ArrowRight className="w-3 h-3 translate-x-0 group-hover:translate-x-1 transition-transform duration-200" />
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
