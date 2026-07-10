import { Box, Typography } from "@mui/material";
import { useActivities } from "../../../hooks/useActivities";

export default function ActivityList() {
  const [activities, isPending] = useActivities();

  if (!activities || isPending) {
    return (
      <Typography className="app" style={{ color: "red" }}>
        Reactivities
      </Typography>
    );
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
     
    </Box>
  )
}