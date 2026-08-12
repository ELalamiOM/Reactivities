import { useQuery } from "@tanstack/react-query";
import agent from "../../../api/agent";
import { useAccount } from "./useAccount";

export function useCredits() {
  const { currentUser } = useAccount();

  const { data: account, isPending } = useQuery({
    queryKey: ["credits"],
    queryFn: async () => {
      const response = await agent.get<PrepaidAccount>("/api/accounts/me");
      return response.data;
    },
    enabled: !!currentUser, // pas d'appel si non connecté
    staleTime: 30_000,
  });

  return {
    balance: account?.balance ?? 0,
    isLoadingCredits: isPending,
  };
}