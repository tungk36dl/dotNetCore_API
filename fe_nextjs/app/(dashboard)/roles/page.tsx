"use client";

import { useEffect, useState, useCallback } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import toast from "react-hot-toast";
import { Plus, Search, Pencil, Trash2, Shield } from "lucide-react";
import { motion, AnimatePresence } from "motion/react";
import { roleService, RoleSearchParams } from "@/services/role.service";
import type { Role } from "@/types/role";
import type { PagedResult } from "@/types/api";
import { Header } from "@/components/layout/header";
import { Button } from "@/components/ui/button";
import { Pagination } from "@/components/ui/pagination";
import { TableRowSkeleton } from "@/components/ui/skeleton";
import { staggerContainer, staggerItem } from "@/components/motion/variants";

export default function RolesPage() {
  const router = useRouter();
  const [data, setData] = useState<PagedResult<Role> | null>(null);
  const [loading, setLoading] = useState(true);
  const [keyword, setKeyword] = useState("");
  const [page, setPage] = useState(1);

  const fetchRoles = useCallback(async () => {
    setLoading(true);
    try {
      const params: RoleSearchParams = {
        keyword: keyword || undefined,
        page,
        pageSize: 10,
      };
      const res = await roleService.getAll(params);
      if (res.success) setData(res.data);
    } catch {
      toast.error("Failed to load roles");
    } finally {
      setLoading(false);
    }
  }, [keyword, page]);

  useEffect(() => { fetchRoles(); }, [fetchRoles]);

  const handleDelete = async (id: string, name: string) => {
    if (!confirm(`Delete role "${name}"?`)) return;
    try {
      await roleService.delete(id);
      toast.success("Role deleted");
      fetchRoles();
    } catch {
      toast.error("Failed to delete role");
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 12 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.35, ease: [0.22, 1, 0.36, 1] }}
    >
      <Header subtitle="Management" title="Roles">
        <Link href="/roles/create">
          <Button size="sm">
            <Plus className="w-3.5 h-3.5 mr-1.5" />
            New Role
          </Button>
        </Link>
      </Header>

      {/* Search */}
      <form
        onSubmit={(e) => { e.preventDefault(); setPage(1); fetchRoles(); }}
        className="flex gap-3 mb-7"
      >
        <div className="flex-1 max-w-sm relative">
          <Search className="absolute left-0 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-ash pointer-events-none" />
          <input
            className="w-full bg-transparent pl-5 py-2 text-sm text-chalk border-b border-rim focus:border-b-gold outline-none placeholder:text-ash transition-colors"
            placeholder="Search roles…"
            value={keyword}
            onChange={(e) => setKeyword(e.target.value)}
          />
        </div>
        <Button type="submit" variant="secondary" size="sm">Search</Button>
        {keyword && (
          <Button type="button" variant="ghost" size="sm"
            onClick={() => { setKeyword(""); setPage(1); }}>
            Clear
          </Button>
        )}
      </form>

      {/* Table */}
      <div className="bg-surface border border-rim overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-rim">
                {["Name", "Description", "Permissions"].map((h) => (
                  <th key={h}
                    className="px-5 py-3 text-left text-[10px] uppercase tracking-[0.15em] text-dim">
                    {h}
                  </th>
                ))}
                <th className="px-5 py-3 text-right text-[10px] uppercase tracking-[0.15em] text-dim">
                  Actions
                </th>
              </tr>
            </thead>

            <AnimatePresence mode="wait">
              {loading ? (
                <tbody key="sk">
                  {Array.from({ length: 4 }).map((_, i) => (
                    <TableRowSkeleton key={i} cols={4} />
                  ))}
                </tbody>
              ) : (
                <motion.tbody
                  key="rows"
                  variants={staggerContainer}
                  initial="hidden"
                  animate="visible"
                >
                  {!data?.items?.length ? (
                    <tr>
                      <td colSpan={4} className="py-16 text-center">
                        <div className="flex flex-col items-center gap-3">
                          <Shield className="w-8 h-8 text-ash" />
                          <p className="text-sm text-dim">No roles found</p>
                          {keyword && (
                            <p className="text-xs text-ash">Try a different search term</p>
                          )}
                        </div>
                      </td>
                    </tr>
                  ) : (
                    data.items.map((role) => (
                      <motion.tr
                        key={role.id}
                        variants={staggerItem}
                        className="border-b border-rim/50 hover:bg-surface-hi transition-colors duration-150 group"
                      >
                        <td className="px-5 py-3.5">
                          <span
                            className="text-sm font-semibold text-chalk"
                            style={{ fontFamily: "var(--font-jetbrains)" }}
                          >
                            {role.name}
                          </span>
                        </td>
                        <td className="px-5 py-3.5 text-sm text-mist max-w-xs truncate">
                          {role.description || <span className="text-ash">—</span>}
                        </td>
                        <td className="px-5 py-3.5">
                          <span
                            className="text-[10px] uppercase tracking-widest border border-gold-dim px-2 py-0.5 text-gold"
                            style={{ fontFamily: "var(--font-jetbrains)" }}
                          >
                            {role.permissionIds?.length ?? 0}
                          </span>
                        </td>
                        <td className="px-5 py-3.5">
                          <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity duration-150">
                            <button
                              onClick={() => router.push(`/roles/${role.id}/edit`)}
                              className="w-7 h-7 flex items-center justify-center border border-rim text-dim hover:text-chalk hover:border-rim-hi transition-colors"
                              title="Edit"
                            >
                              <Pencil className="w-3 h-3" />
                            </button>
                            <button
                              onClick={() => handleDelete(role.id, role.name)}
                              className="w-7 h-7 flex items-center justify-center border border-rim text-dim hover:text-danger hover:border-danger/40 transition-colors"
                              title="Delete"
                            >
                              <Trash2 className="w-3 h-3" />
                            </button>
                          </div>
                        </td>
                      </motion.tr>
                    ))
                  )}
                </motion.tbody>
              )}
            </AnimatePresence>
          </table>
        </div>

        {data && (
          <div className="px-5 py-3 border-t border-rim bg-void flex items-center justify-between">
            <span
              className="text-[10px] uppercase tracking-widest text-dim"
              style={{ fontFamily: "var(--font-jetbrains)" }}
            >
              {data.totalCount} role{data.totalCount !== 1 ? "s" : ""}
            </span>
            <Pagination
              pageIndex={data.pageIndex}
              totalPages={data.totalPages}
              hasPreviousPage={data.hasPreviousPage}
              hasNextPage={data.hasNextPage}
              onPageChange={setPage}
            />
          </div>
        )}
      </div>
    </motion.div>
  );
}
