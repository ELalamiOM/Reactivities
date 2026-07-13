import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { LoginSchema } from "../schemas/loginSchema";
import type { RegisterSchema } from "../schemas/registerSchema";
import {
  getCurrentUser,
  loginWithCookies,
  logout,
  registerUser,
  forgotPassword as forgotPasswordApi,
} from "../api/agent";
import type { ForgotPasswordSchema } from "../schemas/forgotPasswordSchema";

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

  const registerAccount = useMutation({
    mutationFn: async (creds: RegisterSchema) => {
      return await registerUser(creds.email, creds.displayName, creds.password);
    },
    onSuccess: (user) => {
      queryClient.setQueryData(['user'], user);
    }
  });

  const logoutUser = useMutation({
    mutationFn: async () => {
      return await logout();
    },
    onSuccess: () => {
      queryClient.setQueryData(['user'], null);
    }
  });

  const { data: currentUser } = useQuery({
    queryKey: ['user'],
    queryFn: async () => {
      return await getCurrentUser();
    },
    retry: false,
    staleTime: 1000 * 60 * 5,
  });

  const forgotPassword = useMutation({
    mutationFn: async (creds: ForgotPasswordSchema) => {
      return await forgotPasswordApi(creds.email);
    },
  });

  return {
    loginUser,
    registerAccount,
    logoutUser,
    currentUser,
    forgotPassword
  };
};