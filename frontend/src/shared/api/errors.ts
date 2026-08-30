import axios, { AxiosError } from 'axios';
import type { ApiResponse } from '@/shared/types/api';

const DEFAULT_ERROR_MESSAGE =
  'Không thể kết nối đến hệ thống. Vui lòng thử lại sau.';

function isApiError(
  error: unknown,
): error is AxiosError<ApiResponse<unknown>> {
  return axios.isAxiosError<ApiResponse<unknown>>(error);
}

export function getApiMessage(error: unknown) {
  if (!isApiError(error)) {
    return DEFAULT_ERROR_MESSAGE;
  }

  return (
    error.response?.data?.message ??
    error.response?.data?.errorCode ??
    error.message ??
    DEFAULT_ERROR_MESSAGE
  );
}
