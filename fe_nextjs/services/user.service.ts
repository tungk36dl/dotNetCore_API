import api from "@/lib/api";
import type { ApiResponse, PagedResult } from "@/types/api";
import type { User, CreateUserRequest, UpdateUserRequest } from "@/types/user";

export interface UserSearchParams {
  keyword?: string;
  userName?: string;
  email?: string;
  fullName?: string;
  gender?: string;
  roleId?: string;
  sortBy?: string;
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}

export const userService = {
  getAll: async (params: UserSearchParams = {}) => {
    const response = await api.get<ApiResponse<PagedResult<User>>>(
      "/api/users",
      { params }
    );
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<ApiResponse<User>>(`/api/users/${id}`);
    return response.data;
  },

  create: async (data: CreateUserRequest) => {
    const response = await api.post<ApiResponse<{ id: string }>>(
      "/api/users",
      data
    );
    return response.data;
  },

  update: async (id: string, data: UpdateUserRequest) => {
    const response = await api.put<ApiResponse>(`/api/users/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    const response = await api.delete<ApiResponse>(`/api/users/${id}`);
    return response.data;
  },

  getRoles: async () => {
    const response = await api.get<
      ApiResponse<Array<{ id: string; name: string; description?: string }>>
    >("/api/users/roles");
    return response.data;
  },
};
