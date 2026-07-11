import CalendarMonthIcon from "@mui/icons-material/CalendarMonth";
import InfoIcon from "@mui/icons-material/Info";
import PlaceIcon from "@mui/icons-material/Place";
import PersonIcon from "@mui/icons-material/Person";
import {
  Avatar,
  Box,
  Button,
  Grid,
  Paper,
  Typography,
} from "@mui/material";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import agent from "../../../api/agent";
import { useAccount } from "../../../hooks/useAccount";

export default function ActivityDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { currentUser } = useAccount();
  const [showMap, setShowMap] = useState(true);

  const { data: activity, isPending } = useQuery({
    queryKey: ["activity", id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/api/activities/${id}`);
      return response.data;
    },
    enabled: !!id,
    retry: false,
  });

  const updateAttendance = useMutation({
    mutationFn: async () => {
      await agent.post(`/api/activities/${id}/attend`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["activity", id] });
      queryClient.invalidateQueries({ queryKey: ["activities"] });
    },
  });

  if (isPending) {
    return <Typography>Loading activity...</Typography>;
  }

  if (!activity) {
    return <Typography color="error">Activity not found.</Typography>;
  }

  const isGoing = activity.attendees?.some((a) => a.id === currentUser?.id);
  const isHost = activity.hostId === currentUser?.id;

  return (
    <Grid container spacing={3}>
      <Grid size={8}>
        <Paper sx={{ overflow: "hidden", borderRadius: 2 }}>
          <Box
            sx={{
              position: "relative",
              minHeight: 380,
              backgroundImage:
                "linear-gradient(to top, rgba(0,0,0,0.8), rgba(0,0,0,0.15)), url('/images/categoryImages/culture.png')",
              backgroundSize: "cover",
              backgroundPosition: "center",
              p: 3,
              display: "flex",
              alignItems: "flex-end",
            }}
          >
            <Box sx={{ color: "white" }}>
              <Typography variant="h3" sx={{ fontWeight: "bold" }}>
                {activity.title}
              </Typography>
              <Typography variant="h6">{activity.date}</Typography>
              <Typography variant="h6">
                Hosted by <b>{activity.hostDisplayName}</b>
              </Typography>
            </Box>
            {!isHost && (
              <Button
                variant="contained"
                color={isGoing ? "warning" : "primary"}
                disabled={!currentUser || updateAttendance.isPending}
                onClick={() => updateAttendance.mutate()}
                sx={{ position: "absolute", right: 16, bottom: 16 }}
              >
                {isGoing ? "Cancel attendance" : "Join Activity"}
              </Button>
            )}
          </Box>
        </Paper>

        <Paper sx={{ mt: 2, borderRadius: 2, overflow: "hidden" }}>
          <Box sx={{ p: 2, borderBottom: "1px solid", borderColor: "divider", display: "flex", gap: 2 }}>
            <InfoIcon color="primary" />
            <Typography>{activity.description}</Typography>
          </Box>
          <Box sx={{ p: 2, borderBottom: "1px solid", borderColor: "divider", display: "flex", gap: 2 }}>
            <CalendarMonthIcon color="primary" />
            <Typography>{activity.date}</Typography>
          </Box>
          <Box sx={{ p: 2, borderBottom: "1px solid", borderColor: "divider", display: "flex", alignItems: "center", gap: 2 }}>
            <PlaceIcon color="primary" />
            <Typography sx={{ flex: 1 }}>
              {activity.venue}, {activity.city}
            </Typography>
            <Button onClick={() => setShowMap((value) => !value)}>
              {showMap ? "Hide Map" : "Show Map"}
            </Button>
          </Box>
          {showMap && (
            <Box sx={{ height: 280 }}>
              <iframe
                title="activity-location"
                width="100%"
                height="100%"
                style={{ border: 0 }}
                loading="lazy"
                src={`https://www.openstreetmap.org/export/embed.html?bbox=${activity.longitude - 0.05}%2C${activity.latitude - 0.05}%2C${activity.longitude + 0.05}%2C${activity.latitude + 0.05}&layer=mapnik&marker=${activity.latitude}%2C${activity.longitude}`}
              />
            </Box>
          )}
        </Paper>

        <Box sx={{ mt: 2 }}>
          <Button onClick={() => navigate("/activities")}>Back to activities</Button>
        </Box>
      </Grid>

      <Grid size={4}>
        <Paper sx={{ borderRadius: 2, overflow: "hidden" }}>
          <Box sx={{ bgcolor: "primary.main", color: "white", p: 2, textAlign: "center" }}>
            <Typography variant="h6">
              {activity.attendees?.length ?? 0} people going
            </Typography>
          </Box>
          {activity.attendees?.map((attendee) => (
            <Box key={attendee.id} sx={{ p: 2, display: "flex", alignItems: "center", gap: 2 }}>
              <Avatar src={attendee.imageUrl} sx={{ width: 56, height: 56 }}>
                <PersonIcon />
              </Avatar>
              <Typography variant="h6" sx={{ color: "primary.main", flex: 1 }}>
                {attendee.displayName}
              </Typography>
              {attendee.id === activity.hostId && (
                <Button size="small" color="warning" variant="contained">
                  Host
                </Button>
              )}
            </Box>
          ))}
        </Paper>
      </Grid>
    </Grid>
  );
}
