"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
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
import { FormCard, FormField, SelectField } from "@/components/ui/form-card";
import { FormSkeleton } from "@/components/ui/skeleton";

const schema = z.object({
  fullName:    z.string().optional(),
  phoneNumber: z.string().optional(),
  gender:      z.string().optional(),
  dateOfBirth: z.string().optional(),
  address:     z.string().optional(),
  avatarUrl:   z.string().optional(),
});

type FormData = z.infer<typeof schema>;

export default function EditUserPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(true);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  useEffect(() => {
    const load = async () => {
      try {
        const res = await userService.getById(id);
        if (res.success) {
          reset({
            fullName:    res.data.fullName    || "",
            phoneNumber: res.data.phoneNumber || "",
            gender:      res.data.gender      || "",
            dateOfBirth: res.data.dateOfBirth || "",
            address:     res.data.address     || "",
            avatarUrl:   res.data.avatarUrl   || "",
          });
        }
      } catch {
        toast.error("Failed to load user");
        router.push("/users");
      } finally {
        setFetching(false);
      }
    };
    load();
  }, [id, reset, router]);

  const onSubmit = async (data: FormData) => {
    setLoading(true);
    try {
      const res = await userService.update(id, data);
      if (res.success) {
        toast.success("User updated");
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
      <Header subtitle="Users / Edit" title="Edit User" />

      {fetching ? (
        <div className="max-w-lg bg-surface border border-rim p-7">
          <FormSkeleton fields={5} />
        </div>
      ) : (
        <FormCard>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-7">
            <FormField>
              <Input label="Full Name" placeholder="John Doe"
                error={errors.fullName?.message} {...register("fullName")} />
            </FormField>
            <FormField>
              <Input label="Phone Number" placeholder="+84 xxx xxx xxx"
                error={errors.phoneNumber?.message} {...register("phoneNumber")} />
            </FormField>
            <FormField>
              <SelectField label="Gender" error={errors.gender?.message} {...register("gender")}>
                <option value="" className="bg-surface">Select gender</option>
                <option value="Male"   className="bg-surface">Male</option>
                <option value="Female" className="bg-surface">Female</option>
                <option value="Other"  className="bg-surface">Other</option>
              </SelectField>
            </FormField>
            <FormField>
              <Input label="Date of Birth" type="date"
                error={errors.dateOfBirth?.message} {...register("dateOfBirth")} />
            </FormField>
            <FormField>
              <Input label="Address" placeholder="123 Street, City"
                error={errors.address?.message} {...register("address")} />
            </FormField>
            <FormField>
              <div className="flex items-center gap-3 pt-3">
                <Button type="submit" loading={loading}>Save Changes</Button>
                <Button type="button" variant="ghost" onClick={() => router.push("/users")}
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
