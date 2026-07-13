import axios, { AxiosError } from 'axios';
import type { InternalAxiosRequestConfig } from 'axios';
import { toast } from 'react-toastify';
import { router } from '../app/router/Routes';

const TOKEN_STORAGE_KEY = 'jwt';

export const getToken = () => localStorage.getItem(TOKEN_STORAGE_KEY);

export const setToken = (token: string) => {
  localStorage.setItem(TOKEN_STORAGE_KEY, token);
};

export const clearToken = () => {
  localStorage.removeItem(TOKEN_STORAGE_KEY);
};

const agent = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  withCredentials: true,
});

agent.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

let refreshRequest: Promise<User | null> | null = null;

const refreshAccessToken = async () => {
  if (!refreshRequest) {
    refreshRequest = agent
      .post<User>('/api/account/refresh-token')
      .then((response) => {
        setToken(response.data.token);
        return response.data;
      })
      .catch(() => {
        clearToken();
        return null;
      })
      .finally(() => {
        refreshRequest = null;
      });
  }
  return refreshRequest;
};

type RetryRequestConfig = InternalAxiosRequestConfig & { _retry?: boolean };

agent.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryRequestConfig | undefined;
    const { data, status } = error.response || {};
    const isRefreshCall = originalRequest?.url?.includes('/api/account/refresh-token');
    const isAuthCall =
      originalRequest?.url?.includes('/api/account/login') ||
      originalRequest?.url?.includes('/api/account/register');

    if (status === 401 && originalRequest && !originalRequest._retry && !isRefreshCall && !isAuthCall) {
      originalRequest._retry = true;
      const refreshedUser = await refreshAccessToken();

      if (refreshedUser) {
        originalRequest.headers.Authorization = `Bearer ${refreshedUser.token}`;
        return agent(originalRequest);
      }
    }

    switch (status) {
      case 400:
        if ((data as { errors?: Record<string, string[]> }).errors) {
          const modelStateErrors: string[] = [];
          const errors = (data as { errors: Record<string, string[]> }).errors;
          for (const key in errors) {
            if (errors[key]) {
              modelStateErrors.push(...errors[key]);
            }
          }
          throw modelStateErrors;
        }
        toast.error('Bad request');
        break;
      case 401:
        toast.error('Unauthorized');
        break;
      case 403:
        toast.error('Forbidden');
        break;
      case 404:
        router.navigate('/not-found');
        break;
      case 500:
        router.navigate('/server-error');
        break;
      default:
        break;
    }

    return Promise.reject(error);
  }
);

export const loginWithCookies = async (email: string, password: string) => {
  const payload = { email, password };
  const response = await agent.post<User>('/api/account/login', payload);
  setToken(response.data.token);
  toast.success('Login successful');
  return response.data;
};

export const registerUser = async (email: string, displayName: string, password: string) => {
  const payload = { email, displayName, password };
  const response = await agent.post<User>('/api/account/register', payload);
  setToken(response.data.token);
  return response.data;
};

export const logout = async () => {
  try {
    await agent.post('/api/account/logout');
  } finally {
    clearToken();
  }
  toast.success('Logged out successfully');
};

export const getCurrentUser = async () => {
  if (!getToken()) return null;

  try {
    const response = await agent.get<User>('/api/account/user-info');
    setToken(response.data.token);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 401) {
      clearToken();
    }
    return null;
  }
};

export const forgotPassword = async (email: string) => {
  await agent.post('/api/account/forgot-password', { email });
};

export default agent;
