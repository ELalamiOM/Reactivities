import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { ForgotPasswordSchema } from "../schemas/forgotPasswordSchema";
import type { LoginSchema } from "../schemas/loginSchema";
import type { RegisterSchema } from "../schemas/registerSchema";
import {
  getCurrentUser,
  getSessionInfo,
  loginWithCookies,
  logout,
  requestPasswordReset,
  registerUser,
  validateSession,
} from "../api/agent";

export const useAccount = () => {
  const queryClient = useQueryClient();

  const loginUser = useMutation({
    mutationFn: async (creds: LoginSchema) => {
      return await loginWithCookies(creds.email, creds.password);
    },
    onSuccess: (user) => {
      queryClient.setQueryData(['user'], user);
    }
  });

  const validateUserSession = useMutation({
    mutationFn: async () => {
      return await validateSession();
    }
  });

  const registerAccount = useMutation({
    mutationFn: async (creds: RegisterSchema) => {
      return await registerUser(creds.email, creds.displayName, creds.password);
    },
  });

  const forgotPassword = useMutation({
    mutationFn: async (payload: ForgotPasswordSchema) => {
      return await requestPasswordReset(payload.email);
    },
  });

  const logoutUser = useMutation({
    mutationFn: async () => {
      return await logout();
    },
    onSuccess: () => {
      queryClient.setQueryData(['user'], null);
    }
  });

  const getUserSession = useMutation({
    mutationFn: async () => {
      return await getSessionInfo();
    }
  });

  const { data: currentUser } = useQuery({
    queryKey: ['user'],
    queryFn: async () => {
      return await getCurrentUser();
    },
    retry: false,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });

  return {
    loginUser,
    registerAccount,
    forgotPassword,
    validateUserSession,
    logoutUser,
    getUserSession,
    currentUser
  };
};