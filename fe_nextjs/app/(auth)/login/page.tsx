"use client";

import { Suspense, useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import toast from "react-hot-toast";
import { authService } from "@/services/auth.service";
import { useAuthStore } from "@/stores/auth-store";
import { extractErrorMessage } from "@/lib/error";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { PageLoading } from "@/components/ui/loading";

const loginSchema = z.object({
  userNameOrEmail: z.string().min(1, "Username or email is required"),
  password: z.string().min(1, "Password is required"),
});

type LoginForm = z.infer<typeof loginSchema>;

function LoginForm_() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const returnUrl = searchParams.get("returnUrl") || "/";
  const { isAuthenticated, setAuth } = useAuthStore();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isAuthenticated) {
      router.replace(returnUrl);
    }
  }, [isAuthenticated, returnUrl, router]);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({ resolver: zodResolver(loginSchema) });

  const onSubmit = async (data: LoginForm) => {
    setLoading(true);
    try {
      const response = await authService.login(data);
      if (response.success) {
        const { accessToken, refreshToken, user } = response.data;
        setAuth(user, accessToken, refreshToken);
        document.cookie = `auth-token=${accessToken}; path=/; max-age=${60 * 60 * 24 * 7}`;
        toast.success("Access granted");
        router.push(returnUrl);
      } else {
        toast.error(response.message || "Authentication failed");
      }
    } catch (error: unknown) {
      toast.error(extractErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-void flex">
      {/* Left decorative panel */}
      <div className="hidden lg:flex w-1/2 relative flex-col justify-between p-12 border-r border-rim overflow-hidden">
        {/* Background grid lines */}
        <div
          className="absolute inset-0 opacity-[0.03]"
          style={{
            backgroundImage:
              "linear-gradient(var(--chalk) 1px, transparent 1px), linear-gradient(90deg, var(--chalk) 1px, transparent 1px)",
            backgroundSize: "48px 48px",
          }}
        />

        {/* Corner accent */}
        <div className="absolute top-0 left-0 w-24 h-24">
          <div className="absolute top-0 left-0 w-full h-px bg-gold opacity-40" />
          <div className="absolute top-0 left-0 h-full w-px bg-gold opacity-40" />
        </div>
        <div className="absolute bottom-0 right-0 w-24 h-24">
          <div className="absolute bottom-0 right-0 w-full h-px bg-gold opacity-40" />
          <div className="absolute bottom-0 right-0 h-full w-px bg-gold opacity-40" />
        </div>

        {/* Top label */}
        <div className="relative z-10">
          <span className="text-[10px] uppercase tracking-[0.25em] text-gold">
            ProjectCore
          </span>
        </div>

        {/* Big decorative text */}
        <div className="relative z-10 select-none">
          <p
            className="text-[120px] leading-none font-light text-chalk opacity-[0.04] absolute -top-10 -left-4 pointer-events-none"
            style={{ fontFamily: "var(--font-cormorant)" }}
          >
            PC
          </p>
          <h2
            className="text-5xl leading-tight text-chalk"
            style={{ fontFamily: "var(--font-cormorant)", fontWeight: 300 }}
          >
            Administration
            <br />
            <span className="text-gold italic">Console</span>
          </h2>
          <p className="mt-4 text-sm text-dim max-w-xs leading-relaxed">
            Manage users, roles, and permissions with precision and clarity.
          </p>
        </div>

        {/* Bottom system info */}
        <div
          className="relative z-10 text-[10px] text-ash space-y-1"
          style={{ fontFamily: "var(--font-jetbrains)" }}
        >
          <p>SYS · v2025.1</p>
          <p>STATUS · ONLINE</p>
        </div>
      </div>

      {/* Right login panel */}
      <div className="flex-1 flex items-center justify-center p-8 lg:p-16">
        <div className="w-full max-w-sm animate-fade-in">
          {/* Mobile logo */}
          <div className="lg:hidden mb-10 flex items-center gap-2.5">
            <div className="w-7 h-7 bg-gold-ghost border border-gold-dim flex items-center justify-center">
              <span
                className="text-gold text-xs font-bold"
                style={{ fontFamily: "var(--font-jetbrains)" }}
              >
                PC
              </span>
            </div>
            <span
              className="text-chalk text-lg"
              style={{ fontFamily: "var(--font-cormorant)", fontWeight: 600 }}
            >
              ProjectCore
            </span>
          </div>

          {/* Heading */}
          <div className="mb-10">
            <p className="text-[10px] uppercase tracking-[0.2em] text-gold mb-3">
              Authentication
            </p>
            <h1
              className="text-4xl text-chalk leading-none"
              style={{ fontFamily: "var(--font-cormorant)", fontWeight: 600 }}
            >
              Sign In
            </h1>
            <p className="text-sm text-dim mt-2">
              Enter your credentials to access the console.
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-7">
            <Input
              label="Username or Email"
              placeholder="admin@example.com"
              error={errors.userNameOrEmail?.message}
              {...register("userNameOrEmail")}
            />

            <Input
              label="Password"
              type="password"
              placeholder="••••••••"
              error={errors.password?.message}
              {...register("password")}
            />

            <div className="pt-2">
              <Button
                type="submit"
                loading={loading}
                className="w-full"
                size="lg"
              >
                {loading ? "Authenticating" : "Sign In"}
              </Button>
            </div>
          </form>

          {/* Footer */}
          <p
            className="mt-10 text-[10px] text-ash text-center tracking-widest uppercase"
            style={{ fontFamily: "var(--font-jetbrains)" }}
          >
            ProjectCore · Secure Access
          </p>
        </div>
      </div>
    </div>
  );
}

export default function LoginPage() {
  return (
    <Suspense fallback={<PageLoading />}>
      <LoginForm_ />
    </Suspense>
  );
}
