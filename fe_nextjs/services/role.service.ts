import api from "@/lib/api";
import type { ApiResponse, PagedResult } from "@/types/api";
import type { Role, CreateRoleRequest, UpdateRoleRequest } from "@/types/role";

export interface RoleSearchParams {
  keyword?: string;
  name?: string;
  sortBy?: string;
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}

export const roleService = {
  getAll: async (params: RoleSearchParams = {}) => {
    const response = await api.get<ApiResponse<PagedResult<Role>>>(
      "/api/roles",
      { params }
    );
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<ApiResponse<Role>>(`/api/roles/${id}`);
    return response.data;
  },

  create: async (data: CreateRoleRequest) => {
    const response = await api.post<ApiResponse<{ id: string }>>(
      "/api/roles",
      data
    );
    return response.data;
  },

  update: async (id: string, data: UpdateRoleRequest) => {
    const response = await api.put<ApiResponse>(`/api/roles/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    const response = await api.delete<ApiResponse>(`/api/roles/${id}`);
    return response.data;
  },
};
