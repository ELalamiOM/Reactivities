import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import type { ForgotPasswordSchema } from "../schemas/forgotPasswordSchema";
import type { LoginSchema } from "../schemas/loginSchema";
import type { RegisterSchema } from "../schemas/registerSchema";
import agent, {
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
    onSuccess: () => {
      console.log('[useAccount] Login successful');
      queryClient.invalidateQueries({ queryKey: ['user'] });
    },
    onError: (error) => {
      console.error('[useAccount] Login error:', error);
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
      queryClient.removeQueries({ queryKey: ['user'] });
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
      const response = await agent.get('/api/account/user-info');
      if (response.status === 204) return null;
      return response.data;
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