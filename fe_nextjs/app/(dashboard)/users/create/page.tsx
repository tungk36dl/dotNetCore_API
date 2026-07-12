"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import toast from "react-hot-toast";
import { motion } from "motion/react";
import { ArrowLeft } from "lucide-react";
import { userService } from "@/services/user.service";
import { extractErrorMessage } from "@/lib/error";
import { Header } from "@/components/layout/header";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FormCard, FormField } from "@/components/ui/form-card";

const schema = z.object({
  userName: z.string().min(3, "Min 3 characters").max(50),
  email:    z.string().email("Invalid email"),
  password: z.string().min(6, "Min 6 characters"),
});

type FormData = z.infer<typeof schema>;

export default function CreateUserPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: FormData) => {
    setLoading(true);
    try {
      const res = await userService.create(data);
      if (res.success) {
        toast.success("User created");
        router.push("/users");
      } else {
        toast.error(res.message);
      }
    } catch (e) {
      toast.error(extractErrorMessage(e));
    } finally {
      setLoading(false);
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 12 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.35, ease: [0.22, 1, 0.36, 1] }}
    >
      <Header subtitle="Users / New" title="Create User" />

      <FormCard>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-7">
          <FormField>
            <Input label="Username" placeholder="johndoe"
              error={errors.userName?.message} {...register("userName")} />
          </FormField>
          <FormField>
            <Input label="Email" type="email" placeholder="john@example.com"
              error={errors.email?.message} {...register("email")} />
          </FormField>
          <FormField>
            <Input label="Password" type="password" placeholder="••••••"
              error={errors.password?.message} {...register("password")} />
          </FormField>
          <FormField>
            <div className="flex items-center gap-3 pt-3">
              <Button type="submit" loading={loading}>Create User</Button>
              <Button type="button" variant="ghost" onClick={() => router.push("/users")}
                className="gap-1.5">
                <ArrowLeft className="w-3.5 h-3.5" />Cancel
              </Button>
            </div>
          </FormField>
        </form>
      </FormCard>
    </motion.div>
  );
}
