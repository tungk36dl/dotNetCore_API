import api from "@/lib/api";
import type { ApiResponse } from "@/types/api";
import type { LoginRequest, LoginResponse, AuthUser } from "@/types/auth";

export const authService = {
  login: async (data: LoginRequest) => {
    const response = await api.post<ApiResponse<LoginResponse>>(
      "/api/auth/login",
      data
    );
    return response.data;
  },

  me: async () => {
    const response = await api.get<ApiResponse<AuthUser>>("/api/auth/me");
    return response.data;
  },

  logout: async (refreshToken: string | null) => {
    const response = await api.post<ApiResponse>("/api/auth/logout", {
      refreshToken,
    });
    return response.data;
  },

  refresh: async (accessToken: string, refreshToken: string) => {
    const response = await api.post<
      ApiResponse<{ accessToken: string; refreshToken: string }>
    >("/api/auth/refresh", {
      accessToken,
      refreshToken,
    });
    return response.data;
  },
};
