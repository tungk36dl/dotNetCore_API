"use client";

import { ButtonHTMLAttributes, forwardRef } from "react";
import { Loader2 } from "lucide-react";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary" | "danger" | "ghost";
  size?: "sm" | "md" | "lg";
  loading?: boolean;
}

const variants: Record<string, string> = {
  primary:
    "bg-gold text-void hover:bg-gold-hi border border-gold hover:border-gold-hi font-semibold tracking-wide",
  secondary:
    "bg-transparent text-mist border border-rim hover:border-rim-hi hover:text-chalk",
  danger:
    "bg-danger-dim text-danger border border-danger/30 hover:bg-danger hover:text-void hover:border-danger",
  ghost:
    "bg-transparent text-dim hover:text-chalk border border-transparent hover:border-rim",
};

const sizes: Record<string, string> = {
  sm: "px-3 py-1.5 text-xs",
  md: "px-4 py-2 text-sm",
  lg: "px-6 py-2.5 text-sm",
};

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      variant = "primary",
      size = "md",
      loading = false,
      disabled,
      children,
      className = "",
      ...props
    },
    ref
  ) => {
    return (
      <button
        ref={ref}
        disabled={disabled || loading}
        className={`
          inline-flex items-center justify-center
          transition-all duration-150 cursor-pointer
          focus:outline-none focus-visible:ring-1 focus-visible:ring-gold focus-visible:ring-offset-1 focus-visible:ring-offset-void
          disabled:opacity-40 disabled:cursor-not-allowed
          uppercase tracking-widest
          ${variants[variant]}
          ${sizes[size]}
          ${className}
        `}
        {...props}
      >
        {loading && (
          <Loader2 className="w-3.5 h-3.5 mr-2 animate-spin" />
        )}
        {children}
      </button>
    );
  }
);

Button.displayName = "Button";
