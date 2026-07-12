"use client";

/** Skeleton pulse line — matches Dark Obsidian Console aesthetic */
export function Skeleton({ className = "" }: { className?: string }) {
  return (
    <div
      className={`bg-surface-hi animate-pulse ${className}`}
      style={{ borderRadius: 0 }}
    />
  );
}

/** Full-width skeleton row for tables */
export function TableRowSkeleton({ cols = 5 }: { cols?: number }) {
  return (
    <tr className="border-b border-rim">
      {Array.from({ length: cols }).map((_, i) => (
        <td key={i} className="px-5 py-4">
          <Skeleton className={`h-3 ${i === 0 ? "w-28" : i === cols - 1 ? "w-12 ml-auto" : "w-36"}`} />
        </td>
      ))}
    </tr>
  );
}

/** Form skeleton — stacked label + input lines */
export function FormSkeleton({ fields = 4 }: { fields?: number }) {
  return (
    <div className="space-y-7">
      {Array.from({ length: fields }).map((_, i) => (
        <div key={i}>
          <Skeleton className="h-2 w-20 mb-3" />
          <Skeleton className="h-px w-full" style={{ height: "1px" }} />
          <Skeleton className="h-7 w-full" />
        </div>
      ))}
    </div>
  );
}
