"use client";

import { motion } from "motion/react";
import { staggerContainer, fieldReveal } from "@/components/motion/variants";

interface FormCardProps {
  children: React.ReactNode;
  className?: string;
}

/** Animated form wrapper — stagger reveals each direct child */
export function FormCard({ children, className = "" }: FormCardProps) {
  return (
    <motion.div
      className={`max-w-lg bg-surface border border-rim p-7 ${className}`}
      variants={staggerContainer}
      initial="hidden"
      animate="visible"
    >
      {children}
    </motion.div>
  );
}

/** Single animated form field wrapper */
export function FormField({ children }: { children: React.ReactNode }) {
  return (
    <motion.div variants={fieldReveal}>
      {children}
    </motion.div>
  );
}

/** Animated select — matches Input underline style */
export function SelectField({
  label,
  error,
  children,
  ...props
}: React.SelectHTMLAttributes<HTMLSelectElement> & { label?: string; error?: string }) {
  return (
    <div className="w-full">
      {label && (
        <label className="block text-[10px] font-semibold uppercase tracking-[0.15em] text-dim mb-2">
          {label}
        </label>
      )}
      <select
        className={`w-full bg-transparent px-0 py-2 text-sm text-chalk border-b transition-colors duration-150 outline-none
          ${error ? "border-b-danger" : "border-b-rim focus:border-b-gold"}
        `}
        {...props}
      >
        {children}
      </select>
      {error && <p className="mt-1.5 text-[11px] text-danger">{error}</p>}
    </div>
  );
}

/** Animated textarea — matches Input underline style */
export function TextareaField({
  label,
  error,
  ...props
}: React.TextareaHTMLAttributes<HTMLTextAreaElement> & { label?: string; error?: string }) {
  return (
    <div className="w-full">
      {label && (
        <label className="block text-[10px] font-semibold uppercase tracking-[0.15em] text-dim mb-2">
          {label}
        </label>
      )}
      <textarea
        className={`w-full bg-transparent px-0 py-2 text-sm text-chalk border-b transition-colors duration-150 outline-none resize-none placeholder:text-ash
          ${error ? "border-b-danger" : "border-b-rim focus:border-b-gold"}
        `}
        {...props}
      />
      {error && <p className="mt-1.5 text-[11px] text-danger">{error}</p>}
    </div>
  );
}
