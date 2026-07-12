"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import toast from "react-hot-toast";
import { motion } from "motion/react";
import { ArrowLeft } from "lucide-react";
import { roleService } from "@/services/role.service";
import { extractErrorMessage } from "@/lib/error";
import { Header } from "@/components/layout/header";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { FormCard, FormField, TextareaField } from "@/components/ui/form-card";

const schema = z.object({
  roleName:    z.string().min(1, "Role name is required"),
  description: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

export default function CreateRolePage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: FormData) => {
    setLoading(true);
    try {
      const res = await roleService.create(data);
      if (res.success) {
        toast.success("Role created");
        router.push("/roles");
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
      <Header subtitle="Roles / New" title="Create Role" />

      <FormCard>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-7">
          <FormField>
            <Input label="Role Name" placeholder="ADMIN, EDITOR, VIEWER…"
              error={errors.roleName?.message} {...register("roleName")} />
          </FormField>
          <FormField>
            <TextareaField
              label="Description"
              placeholder="What does this role do?"
              rows={3}
              {...register("description")}
            />
          </FormField>
          <FormField>
            <div className="flex items-center gap-3 pt-3">
              <Button type="submit" loading={loading}>Create Role</Button>
              <Button type="button" variant="ghost" onClick={() => router.push("/roles")}
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
