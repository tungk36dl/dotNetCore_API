"use client";

import { ChevronLeft, ChevronRight } from "lucide-react";

interface PaginationProps {
  pageIndex: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  onPageChange: (page: number) => void;
}

export function Pagination({
  pageIndex,
  totalPages,
  hasPreviousPage,
  hasNextPage,
  onPageChange,
}: PaginationProps) {
  if (totalPages <= 1) return null;

  const pages: number[] = [];
  const start = Math.max(1, pageIndex - 2);
  const end = Math.min(totalPages, pageIndex + 2);
  for (let i = start; i <= end; i++) pages.push(i);

  const btnBase =
    "inline-flex items-center justify-center w-7 h-7 text-xs transition-colors duration-150 border";

  return (
    <div className="flex items-center gap-1">
      <button
        disabled={!hasPreviousPage}
        onClick={() => onPageChange(pageIndex - 1)}
        className={`${btnBase} border-rim text-dim hover:border-rim-hi hover:text-chalk disabled:opacity-30 disabled:cursor-not-allowed`}
      >
        <ChevronLeft className="w-3.5 h-3.5" />
      </button>

      {start > 1 && (
        <>
          <button
            onClick={() => onPageChange(1)}
            className={`${btnBase} border-rim text-dim hover:border-rim-hi hover:text-chalk`}
          >
            1
          </button>
          {start > 2 && (
            <span className="w-7 h-7 flex items-center justify-center text-xs text-ash">
              ···
            </span>
          )}
        </>
      )}

      {pages.map((page) => (
        <button
          key={page}
          onClick={() => onPageChange(page)}
          className={`${btnBase} ${
            page === pageIndex
              ? "border-gold bg-gold text-void font-semibold"
              : "border-rim text-dim hover:border-rim-hi hover:text-chalk"
          }`}
        >
          {page}
        </button>
      ))}

      {end < totalPages && (
        <>
          {end < totalPages - 1 && (
            <span className="w-7 h-7 flex items-center justify-center text-xs text-ash">
              ···
            </span>
          )}
          <button
            onClick={() => onPageChange(totalPages)}
            className={`${btnBase} border-rim text-dim hover:border-rim-hi hover:text-chalk`}
          >
            {totalPages}
          </button>
        </>
      )}

      <button
        disabled={!hasNextPage}
        onClick={() => onPageChange(pageIndex + 1)}
        className={`${btnBase} border-rim text-dim hover:border-rim-hi hover:text-chalk disabled:opacity-30 disabled:cursor-not-allowed`}
      >
        <ChevronRight className="w-3.5 h-3.5" />
      </button>
    </div>
  );
}
