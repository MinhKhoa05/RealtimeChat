export type ApiResponse<T = unknown> = {
  success: boolean;
  message: string | null;
  data: T;
  errorCode?: string;
};
