"use client";

import { useEffect } from "react";
import { Button } from "@/components/ui/button";

interface ErrorProps {
  error: Error & { digest?: string };
  reset: () => void;
}

export default function DashboardError({ error, reset }: ErrorProps) {
  useEffect(() => {
    console.error("[Dashboard Error]", error);
  }, [error]);

  return (
    <div className="flex items-center justify-center py-32 animate-fade-in">
      <div className="text-center max-w-sm">
        <p className="text-[10px] uppercase tracking-[0.25em] text-danger mb-4">
          Error
        </p>
        <h2
          className="text-2xl text-chalk mb-3 leading-none"
          style={{ fontFamily: "var(--font-cormorant)", fontWeight: 600 }}
        >
          Something went wrong
        </h2>
        <p className="text-sm text-dim mb-2 leading-relaxed">
          {error.message || "An unexpected error occurred while loading this page."}
        </p>
        {error.digest && (
          <p
            className="text-[10px] text-ash mb-6"
            style={{ fontFamily: "var(--font-jetbrains)" }}
          >
            Ref: {error.digest}
          </p>
        )}
        <Button variant="secondary" size="sm" onClick={reset}>
          Try Again
        </Button>
      </div>
    </div>
  );
}
