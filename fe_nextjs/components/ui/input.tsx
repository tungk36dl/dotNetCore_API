"use client";

import { InputHTMLAttributes, forwardRef } from "react";

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, className = "", ...props }, ref) => {
    return (
      <div className="w-full">
        {label && (
          <label className="block text-[10px] font-semibold uppercase tracking-[0.15em] text-dim mb-2">
            {label}
          </label>
        )}
        <input
          ref={ref}
          className={`
            w-full bg-transparent
            px-0 py-2
            text-sm text-chalk
            border-b transition-colors duration-150
            outline-none
            placeholder:text-ash
            ${error
              ? "border-b-danger"
              : "border-b-rim focus:border-b-gold"
            }
            ${className}
          `}
          {...props}
        />
        {error && (
          <p className="mt-1.5 text-[11px] text-danger">{error}</p>
        )}
      </div>
    );
  }
);

Input.displayName = "Input";
