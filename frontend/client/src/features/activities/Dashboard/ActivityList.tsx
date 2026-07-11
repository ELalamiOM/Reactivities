import { Box, Typography } from "@mui/material";
import ActivityCard from "./ActivityCard";

type Props = {
  activities: Activity[];
  isPending?: boolean;
};

export default function ActivityList({ activities, isPending = false }: Props) {
  if (isPending) {
    return (
      <Typography className="app" style={{ color: "red" }}>
        Loading activities...
      </Typography>
    );
  }

  if (activities.length === 0) {
    return (
      <Typography sx={{ color: "text.secondary" }}>
        No activities found for this filter.
      </Typography>
    );
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
      {activities.map((activity) => (
        <ActivityCard key={activity.id} activity={activity} />
      ))}
    </Box>
  );
}