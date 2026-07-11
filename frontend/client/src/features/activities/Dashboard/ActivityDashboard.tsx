import FilterListIcon from "@mui/icons-material/FilterList";
import CalendarMonthIcon from "@mui/icons-material/CalendarMonth";
import {
  Grid,
  List,
  ListItemButton,
  ListItemText,
  Paper,
  Typography,
} from "@mui/material";
import { useMemo, useState } from "react";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DateCalendar } from "@mui/x-date-pickers/DateCalendar";
import type { Dayjs } from "dayjs";
import ActivityList from "./ActivityList";
import { useActivities } from "../../../hooks/useActivities";

type ActivityFilter = "all" | "upcoming" | "cancelled";

export default function ActivityDashboard() {
  const [activities, isPending] = useActivities();
  const [activeFilter, setActiveFilter] = useState<ActivityFilter>("all");
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);

  const filteredActivities = useMemo(() => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    return activities.filter((activity) => {
      const activityDate = new Date(activity.date);
      const isUpcoming = activityDate >= today;

      if (activeFilter === "upcoming" && !isUpcoming) return false;
      if (activeFilter === "cancelled" && !activity.isCancelled) return false;

      if (selectedDate) {
        return (
          activityDate.getFullYear() === selectedDate.year() &&
          activityDate.getMonth() === selectedDate.month() &&
          activityDate.getDate() === selectedDate.date()
        );
      }

      return true;
    });
  }, [activities, activeFilter, selectedDate]);

  return (
    <Grid container spacing={3}>
      <Grid size={8}>
        <ActivityList activities={filteredActivities} isPending={isPending} />
      </Grid>
      <Grid size={4}>
        <Paper sx={{ p: 2, borderRadius: 3, mb: 3 }}>
          <Typography
            variant="h5"
            color="primary"
            sx={{ mb: 2, display: "flex", alignItems: "center", gap: 1 }}
          >
            <FilterListIcon />
            Filters
          </Typography>
          <List sx={{ p: 0 }}>
            <ListItemButton
              selected={activeFilter === "all"}
              onClick={() => setActiveFilter("all")}
            >
              <ListItemText primary="All events" />
            </ListItemButton>
            <ListItemButton
              selected={activeFilter === "upcoming"}
              onClick={() => setActiveFilter("upcoming")}
            >
              <ListItemText primary="Upcoming events" />
            </ListItemButton>
            <ListItemButton
              selected={activeFilter === "cancelled"}
              onClick={() => setActiveFilter("cancelled")}
            >
              <ListItemText primary="Cancelled events" />
            </ListItemButton>
          </List>
        </Paper>

        <Paper sx={{ p: 2, borderRadius: 3 }}>
          <Typography
            variant="h5"
            color="primary"
            sx={{ mb: 2, display: "flex", alignItems: "center", gap: 1 }}
          >
            <CalendarMonthIcon />
            Select date
          </Typography>
          <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DateCalendar
              value={selectedDate}
              onChange={(value) => setSelectedDate(value)}
            />
          </LocalizationProvider>
        </Paper>
      </Grid>
    </Grid>
  );
}
