import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";

type PagedResult<T> = {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
};

type UseActivitiesParams = {
  pageNumber?: number;
  pageSize?: number;
  filter?: string;
};

export function useActivities({
  pageNumber = 1,
  pageSize = 10,
  filter,
}: UseActivitiesParams = {}) {
  const queryClient = useQueryClient();

  const { data, isPending } = useQuery({
    queryKey: ["activities", pageNumber, pageSize, filter],
    queryFn: async () => {
      const params = new URLSearchParams();
      params.append("pageNumber", pageNumber.toString());
      params.append("pageSize", pageSize.toString());
      if (filter) params.append("filter", filter);
      const response = await agent.get<PagedResult<Activity>>(
        `/api/activities?${params}`,
      );
      return response.data;
    },
  });

  const deleteActivity = useMutation({
    mutationFn: async (id: string) => {
      await agent.delete(`/api/activities/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["activities"] });
    },
  });

  return {
    activities: data?.items ?? [],
    totalCount: data?.totalCount ?? 0,
    totalPages: data?.totalPages ?? 0,
    pageNumber: data?.pageNumber ?? 1,
    isPending,
    deleteActivity,
  };
}
