// ========== API Response wrapper (khớp với BE ApiResponse<T>) ==========
export interface ApiResponse<T = unknown> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
}

// ========== Pagination (khớp với BE PagedResult<T>) ==========
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// ========== Base audit fields (khớp với BE DomainDto) ==========
export interface DomainDto {
  status: string;
  createdBy?: string;
  createdDate?: string;
  updatedBy?: string;
  updatedDate?: string;
}
