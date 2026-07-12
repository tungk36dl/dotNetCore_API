import type { DomainDto } from "./api";

// ========== User entity (khớp với BE UserDto) ==========
export interface User extends DomainDto {
  id: string;
  userName: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  gender?: string;
  dateOfBirth?: string;
  address?: string;
  avatarUrl?: string;
  roleIds: string[];
}

// ========== Request DTOs (khớp với BE Request models) ==========
export interface CreateUserRequest {
  userName: string;
  email: string;
  password: string;
}

export interface UpdateUserRequest {
  fullName?: string;
  phoneNumber?: string;
  gender?: string;
  dateOfBirth?: string;
  address?: string;
  avatarUrl?: string;
}
