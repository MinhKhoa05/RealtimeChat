import axios from "axios";

export const api = axios.create({
  baseURL: normalizeApiBaseUrl(import.meta.env.VITE_API_BASE_URL),
  withCredentials: true,
});

api.interceptors.request.use((config) => {
  if (config.data instanceof FormData) {
    if (config.headers) {
      delete config.headers["Content-Type"];
    }
    return config;
  }

  if (config.headers && !config.headers["Content-Type"]) {
    config.headers["Content-Type"] = "application/json";
  }

  return config;
});

function normalizeApiBaseUrl(value?: string) {
  const fallback = "http://localhost:5000";
  const base = (value || fallback).trim();
  return base.replace(/\/api\/?$/, "").replace(/\/$/, "");
}
