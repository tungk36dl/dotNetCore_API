"use client";

export function Loading() {
  return (
    <div className="flex items-center justify-center py-16">
      <div className="flex flex-col items-center gap-3">
        <div className="relative w-8 h-8">
          <div className="absolute inset-0 border border-rim rounded-full" />
          <div className="absolute inset-0 border-t border-gold rounded-full animate-spin-slow" />
        </div>
        <span className="text-[10px] uppercase tracking-[0.2em] text-dim animate-pulse-gold">
          Loading
        </span>
      </div>
    </div>
  );
}

export function PageLoading() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-void">
      <div className="flex flex-col items-center gap-4">
        <div className="relative w-12 h-12">
          <div className="absolute inset-0 border border-rim rounded-full" />
          <div className="absolute inset-0 border-t-2 border-gold rounded-full animate-spin-slow" />
        </div>
        <span
          className="text-[11px] uppercase tracking-[0.25em] text-dim animate-pulse-gold"
          style={{ fontFamily: "var(--font-jetbrains)" }}
        >
          ProjectCore
        </span>
      </div>
    </div>
  );
}
