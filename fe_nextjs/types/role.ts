import type { DomainDto } from "./api";

// ========== Role entity (khớp với BE RoleDto) ==========
export interface Role extends DomainDto {
  id: string;
  name: string;
  description?: string;
  permissionIds: string[];
}

// ========== Request DTOs (khớp với BE Request models) ==========
export interface CreateRoleRequest {
  roleName: string;
  description?: string;
}

export interface UpdateRoleRequest {
  name?: string;
  description?: string;
}
