import type { ApiResponse } from "@/types/api";
import axios from "axios";

/**
 * Extract user-friendly error message from any error.
 * Works with:
 * - Axios errors with ApiResponse body (BE validation, domain errors)
 * - Axios errors with non-ApiResponse body (network, etc.)
 * - Plain Error objects
 * - Unknown errors
 */
export function extractErrorMessage(error: unknown): string {
  // Axios error with response from BE
  if (axios.isAxiosError(error) && error.response?.data) {
    const data = error.response.data as ApiResponse;

    // BE ApiResponse format
    if (typeof data.message === "string" && data.message) {
      // If there are validation errors, join them
      if (data.errors?.length) {
        return data.errors.join("; ");
      }
      return data.message;
    }

    // Fallback: ASP.NET ProblemDetails or other format
    if (typeof data === "object" && "title" in data) {
      return (data as { title: string }).title;
    }
  }

  // Axios error without response (network error)
  if (axios.isAxiosError(error) && !error.response) {
    return "Unable to connect to the server";
  }

  // Standard Error
  if (error instanceof Error) {
    return error.message;
  }

  return "An unexpected error occurred";
}

/**
 * Check if an ApiResponse indicates success.
 * Use this instead of manually checking response.success everywhere.
 */
export function isSuccess<T>(response: ApiResponse<T>): response is ApiResponse<T> & { data: T } {
  return response.success === true;
}
