"use client";

import { useEffect, useState, useCallback } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import toast from "react-hot-toast";
import { Plus, Search, Pencil, Trash2, ChevronUp, ChevronDown, Users } from "lucide-react";
import { motion, AnimatePresence } from "motion/react";
import { userService, UserSearchParams } from "@/services/user.service";
import type { User } from "@/types/user";
import type { PagedResult } from "@/types/api";
import { Header } from "@/components/layout/header";
import { Button } from "@/components/ui/button";
import { Pagination } from "@/components/ui/pagination";
import { TableRowSkeleton } from "@/components/ui/skeleton";
import { staggerContainer, staggerItem } from "@/components/motion/variants";

export default function UsersPage() {
  const router = useRouter();
  const [data, setData] = useState<PagedResult<User> | null>(null);
  const [loading, setLoading] = useState(true);
  const [keyword, setKeyword] = useState("");
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState<string | undefined>();
  const [sortDesc, setSortDesc] = useState(false);

  const fetchUsers = useCallback(async () => {
    setLoading(true);
    try {
      const params: UserSearchParams = {
        keyword: keyword || undefined,
        page,
        pageSize: 10,
        sortBy,
        sortDescending: sortDesc,
      };
      const response = await userService.getAll(params);
      if (response.success) setData(response.data);
    } catch {
      toast.error("Failed to load users");
    } finally {
      setLoading(false);
    }
  }, [keyword, page, sortBy, sortDesc]);

  useEffect(() => { fetchUsers(); }, [fetchUsers]);

  const handleDelete = async (id: string, userName: string) => {
    if (!confirm(`Delete user "${userName}"?`)) return;
    try {
      await userService.delete(id);
      toast.success("User deleted");
      fetchUsers();
    } catch {
      toast.error("Failed to delete user");
    }
  };

  const handleSort = (col: string) => {
    if (sortBy === col) setSortDesc(!sortDesc);
    else { setSortBy(col); setSortDesc(false); }
    setPage(1);
  };

  const SortIcon = ({ col }: { col: string }) => {
    if (sortBy !== col) return <span className="text-ash ml-1 opacity-30">⇅</span>;
    return sortDesc
      ? <ChevronDown className="inline w-3 h-3 ml-1 text-gold" />
      : <ChevronUp   className="inline w-3 h-3 ml-1 text-gold" />;
  };

  const cols = [
    { key: "UserName", label: "Username" },
    { key: "Email",    label: "Email"    },
    { key: "FullName", label: "Full Name" },
    { key: "Gender",   label: "Gender"   },
  ] as const;

  return (
    <motion.div
      initial={{ opacity: 0, y: 12 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.35, ease: [0.22, 1, 0.36, 1] }}
    >
      <Header subtitle="Management" title="Users">
        <Link href="/users/create">
          <Button size="sm">
            <Plus className="w-3.5 h-3.5 mr-1.5" />
            New User
          </Button>
        </Link>
      </Header>

      {/* Search */}
      <form
        onSubmit={(e) => { e.preventDefault(); setPage(1); fetchUsers(); }}
        className="flex gap-3 mb-7"
      >
        <div className="flex-1 max-w-sm relative">
          <Search className="absolute left-0 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-ash pointer-events-none" />
          <input
            className="w-full bg-transparent pl-5 py-2 text-sm text-chalk border-b border-rim focus:border-b-gold outline-none placeholder:text-ash transition-colors"
            placeholder="Search by name, email…"
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
                {cols.map((c) => (
                  <th
                    key={c.key}
                    className="px-5 py-3 text-left text-[10px] uppercase tracking-[0.15em] text-dim cursor-pointer hover:text-mist transition-colors select-none"
                    onClick={() => handleSort(c.key)}
                  >
                    {c.label}<SortIcon col={c.key} />
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
                  {Array.from({ length: 6 }).map((_, i) => (
                    <TableRowSkeleton key={i} cols={5} />
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
                      <td colSpan={5} className="py-16 text-center">
                        <div className="flex flex-col items-center gap-3">
                          <Users className="w-8 h-8 text-ash" />
                          <p className="text-sm text-dim">No users found</p>
                          {keyword && (
                            <p className="text-xs text-ash">Try a different search term</p>
                          )}
                        </div>
                      </td>
                    </tr>
                  ) : (
                    data.items.map((user) => (
                      <motion.tr
                        key={user.id}
                        variants={staggerItem}
                        className="border-b border-rim/50 hover:bg-surface-hi transition-colors duration-150 group"
                      >
                        <td className="px-5 py-3.5">
                          <span
                            className="text-sm font-medium text-chalk"
                            style={{ fontFamily: "var(--font-jetbrains)" }}
                          >
                            {user.userName}
                          </span>
                        </td>
                        <td className="px-5 py-3.5 text-sm text-mist">{user.email}</td>
                        <td className="px-5 py-3.5 text-sm text-dim">
                          {user.fullName || <span className="text-ash">—</span>}
                        </td>
                        <td className="px-5 py-3.5 text-sm">
                          {user.gender ? (
                            <span className="text-[10px] uppercase tracking-widest border border-rim-hi px-2 py-0.5 text-mist">
                              {user.gender}
                            </span>
                          ) : (
                            <span className="text-ash">—</span>
                          )}
                        </td>
                        <td className="px-5 py-3.5">
                          <div className="flex items-center justify-end gap-1 opacity-0 group-hover:opacity-100 transition-opacity duration-150">
                            <button
                              onClick={() => router.push(`/users/${user.id}/edit`)}
                              className="w-7 h-7 flex items-center justify-center border border-rim text-dim hover:text-chalk hover:border-rim-hi transition-colors"
                              title="Edit"
                            >
                              <Pencil className="w-3 h-3" />
                            </button>
                            <button
                              onClick={() => handleDelete(user.id, user.userName)}
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
              {data.totalCount} record{data.totalCount !== 1 ? "s" : ""}
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
