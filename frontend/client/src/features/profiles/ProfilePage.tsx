import PersonIcon from "@mui/icons-material/Person";
import {
  Avatar,
  Box,
  Card,
  CardContent,
  CardMedia,
  Grid,
  Paper,
  Tab,
  Tabs,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { useAccount } from "../../hooks/useAccount";
import { useActivities } from "../../hooks/useActivities";

const sideTabs = ["About", "Photos", "Events", "Followers", "Following"];
const eventTabs = ["Future Events", "Past Events", "Hosting"];

export default function ProfilePage() {
  const { currentUser } = useAccount();
  const { activities } = useActivities({ pageSize: 100 });
  const [sideTab, setSideTab] = useState(2);
  const [eventTab, setEventTab] = useState(0);

  const now = new Date();
  const filteredEvents = activities.filter((activity) => {
    if (!currentUser) return false;
    const date = new Date(activity.date);
    const isGoing = activity.attendees?.some((a) => a.id === currentUser.id);
    const isHost = activity.hostId === currentUser.id;
    if (eventTab === 0) return isGoing && date >= now;
    if (eventTab === 1) return isGoing && date < now;
    return isHost;
  });

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Paper sx={{ p: 3, borderRadius: 2 }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 3 }}>
          <Avatar src={currentUser?.imageUrl} sx={{ width: 150, height: 150 }}>
            <PersonIcon sx={{ fontSize: 90 }} />
          </Avatar>
          <Typography variant="h3">
            {currentUser?.displayName ?? "User"}
          </Typography>
        </Box>
      </Paper>

      <Paper sx={{ borderRadius: 2 }}>
        <Grid container>
          <Grid
            size={3}
            sx={{ borderRight: "1px solid", borderColor: "divider" }}
          >
            <Tabs
              orientation="vertical"
              value={sideTab}
              onChange={(_, value) => setSideTab(value)}
              sx={{ py: 2 }}
            >
              {sideTabs.map((label) => (
                <Tab
                  key={label}
                  label={label}
                  sx={{ alignItems: "flex-start" }}
                />
              ))}
            </Tabs>
          </Grid>

          <Grid size={9} sx={{ p: 3 }}>
            {sideTab === 2 ? (
              <>
                <Tabs
                  value={eventTab}
                  onChange={(_, value) => setEventTab(value)}
                  sx={{ mb: 3 }}
                >
                  {eventTabs.map((label) => (
                    <Tab key={label} label={label} />
                  ))}
                </Tabs>

                <Box sx={{ display: "flex", flexWrap: "wrap", gap: 3 }}>
                  {filteredEvents.length === 0 && (
                    <Typography color="text.secondary">
                      No events to display.
                    </Typography>
                  )}
                  {filteredEvents.map((activity) => (
                    <Card
                      key={activity.id}
                      sx={{ width: 180, borderRadius: 2 }}
                    >
                      <CardMedia
                        component="img"
                        height="130"
                        image={`/images/categoryImages/${activity.category.toLowerCase()}.png`}
                        alt={activity.title}
                      />
                      <CardContent sx={{ textAlign: "center" }}>
                        <Typography
                          variant="subtitle1"
                          sx={{ fontWeight: "bold" }}
                        >
                          {activity.title}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          {new Date(activity.date).toLocaleDateString()}
                        </Typography>
                      </CardContent>
                    </Card>
                  ))}
                </Box>
              </>
            ) : (
              <Typography variant="h6" color="text.secondary">
                {sideTabs[sideTab]} content goes here.
              </Typography>
            )}
          </Grid>
        </Grid>
      </Paper>
    </Box>
  );
}
