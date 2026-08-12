import { useMutation, useQueryClient } from "@tanstack/react-query";
import { AxiosError } from "axios";
import agent from "../../../api/agent";

// extrait le message métier renvoyé par Result<T>.Failure
function getApiError(error: unknown, fallback: string) {
  const axiosError = error as AxiosError<string | { detail?: string }>;
  const data = axiosError.response?.data;
  if (typeof data === "string") return data;
  if (data?.detail) return data.detail;
  return fallback;
}

export function useInscriptions(activityId?: string) {
  const queryClient = useQueryClient();

  const invalidate = async () => {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: ["activities"] }),
      queryClient.invalidateQueries({ queryKey: ["activity", activityId] }),
      queryClient.invalidateQueries({ queryKey: ["credits"] }), // 👈 rafraîchit le solde
    ]);
  };

  const register = useMutation({
    mutationFn: async (id: string) => {
      await agent.post(`/api/inscriptions?activityId=${id}`);
    },
    onSuccess: invalidate,
  });

  const unregister = useMutation({
    mutationFn: async (id: string) => {
      await agent.delete(`/api/inscriptions?activityId=${id}`);
    },
    onSuccess: invalidate,
  });

  return { register, unregister, getApiError };
}