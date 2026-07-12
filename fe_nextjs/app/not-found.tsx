import Link from "next/link";

export default function NotFound() {
  return (
    <div className="min-h-screen bg-void flex items-center justify-center px-8">
      <div className="text-center animate-fade-in">
        {/* Decorative number */}
        <p
          className="text-[120px] leading-none font-light text-chalk opacity-[0.03] select-none mb-2"
          style={{ fontFamily: "var(--font-cormorant)" }}
        >
          404
        </p>

        <p className="text-[10px] uppercase tracking-[0.25em] text-gold mb-4">
          Not Found
        </p>

        <h1
          className="text-3xl text-chalk mb-3 leading-none"
          style={{ fontFamily: "var(--font-cormorant)", fontWeight: 600 }}
        >
          Page not found
        </h1>

        <p className="text-sm text-dim mb-8 max-w-xs mx-auto leading-relaxed">
          The resource you are looking for does not exist or has been moved.
        </p>

        <Link
          href="/"
          className="inline-flex items-center gap-2 text-[10px] uppercase tracking-widest text-gold border border-gold-dim px-5 py-2.5 hover:bg-gold-ghost transition-colors duration-150"
        >
          Return to Dashboard
        </Link>
      </div>
    </div>
  );
}
