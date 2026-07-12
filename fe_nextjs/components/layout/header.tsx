"use client";

interface HeaderProps {
  title: string;
  subtitle?: string;
  children?: React.ReactNode;
}

export function Header({ title, subtitle, children }: HeaderProps) {
  return (
    <div className="flex items-end justify-between mb-8 pb-5 border-b border-rim">
      <div>
        {subtitle && (
          <p className="text-[10px] uppercase tracking-[0.2em] text-gold mb-1.5">
            {subtitle}
          </p>
        )}
        <h1
          className="text-3xl text-chalk leading-none"
          style={{ fontFamily: "var(--font-cormorant)", fontWeight: 600 }}
        >
          {title}
        </h1>
      </div>
      {children && (
        <div className="flex items-center gap-2">{children}</div>
      )}
    </div>
  );
}
