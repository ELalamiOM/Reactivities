import { useMutation } from "@tanstack/react-query";
import type { LoginSchema } from "../schemas/loginSchema";

export const useAccount = () => {
  const loginUser = useMutation({
    mutationFn: async (creds: LoginSchema) => {
      const response = await fetch('/login?useCookies=true', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(creds),
        credentials: 'include'
      });

      if (!response.ok) {
        throw new Error('Login failed');
      }

      return response.json();
    }
  });

  return {
    loginUser
  };
};