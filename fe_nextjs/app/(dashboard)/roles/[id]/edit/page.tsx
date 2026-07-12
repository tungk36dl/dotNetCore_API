"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
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
import { FormSkeleton } from "@/components/ui/skeleton";

const schema = z.object({
  name:        z.string().min(1, "Role name is required"),
  description: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

export default function EditRolePage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;
  const [loading, setLoading]   = useState(false);
  const [fetching, setFetching] = useState(true);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  useEffect(() => {
    const load = async () => {
      try {
        const res = await roleService.getById(id);
        if (res.success) {
          reset({
            name:        res.data.name,
            description: res.data.description || "",
          });
        }
      } catch {
        toast.error("Failed to load role");
        router.push("/roles");
      } finally {
        setFetching(false);
      }
    };
    load();
  }, [id, reset, router]);

  const onSubmit = async (data: FormData) => {
    setLoading(true);
    try {
      const res = await roleService.update(id, data);
      if (res.success) {
        toast.success("Role updated");
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
      <Header subtitle="Roles / Edit" title="Edit Role" />

      {fetching ? (
        <div className="max-w-lg bg-surface border border-rim p-7">
          <FormSkeleton fields={2} />
        </div>
      ) : (
        <FormCard>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-7">
            <FormField>
              <Input label="Role Name" placeholder="ADMIN, EDITOR, VIEWER…"
                error={errors.name?.message} {...register("name")} />
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
                <Button type="submit" loading={loading}>Save Changes</Button>
                <Button type="button" variant="ghost" onClick={() => router.push("/roles")}
                  className="gap-1.5">
                  <ArrowLeft className="w-3.5 h-3.5" />Cancel
                </Button>
              </div>
            </FormField>
          </form>
        </FormCard>
      )}
    </motion.div>
  );
}
