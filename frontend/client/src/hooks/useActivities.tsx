import { useQuery } from "@tanstack/react-query";
import agent from "../api/agent";

export function useActivities() {
  const { data = [], isPending } = useQuery({
    queryKey: ["activities"],
    queryFn: async () => {
      const response = await agent.get<Activity[]>("/api/activities");
      return response.data;
    },
  });

  return [data, isPending] as const;
}
