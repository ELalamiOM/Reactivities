import FilterListIcon from "@mui/icons-material/FilterList";
import CalendarMonthIcon from "@mui/icons-material/CalendarMonth";
import {
  Box,
  Grid,
  List,
  ListItemButton,
  ListItemText,
  Pagination,
  Paper,
  Typography,
} from "@mui/material";
import { useState } from "react";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DateCalendar } from "@mui/x-date-pickers/DateCalendar";
import type { Dayjs } from "dayjs";
import ActivityList from "./ActivityList";
import { useActivities } from "../../../hooks/useActivities";

type ActivityFilter = "all" | "upcoming" | "past";

export default function ActivityDashboard() {
  const [activeFilter, setActiveFilter] = useState<ActivityFilter>("all");
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);
  const [page, setPage] = useState(1);

  const serverFilter = activeFilter === "all" ? undefined : activeFilter;
  const { activities, totalPages, isPending } = useActivities({
    pageNumber: page,
    pageSize: 10,
    filter: serverFilter,
  });

  const filteredActivities = selectedDate
    ? activities.filter((activity) => {
        const activityDate = new Date(activity.date);
        return (
          activityDate.getFullYear() === selectedDate.year() &&
          activityDate.getMonth() === selectedDate.month() &&
          activityDate.getDate() === selectedDate.date()
        );
      })
    : activities;

  return (
    <Grid container spacing={3}>
      <Grid size={8}>
        <ActivityList activities={filteredActivities} isPending={isPending} />
        {totalPages > 1 && (
          <Box sx={{ display: "flex", justifyContent: "center", mt: 3 }}>
            <Pagination
              count={totalPages}
              page={page}
              onChange={(_, value) => setPage(value)}
              color="primary"
            />
          </Box>
        )}
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
              onClick={() => {
                setActiveFilter("all");
                setPage(1);
              }}
            >
              <ListItemText primary="All events" />
            </ListItemButton>
            <ListItemButton
              selected={activeFilter === "upcoming"}
              onClick={() => {
                setActiveFilter("upcoming");
                setPage(1);
              }}
            >
              <ListItemText primary="Upcoming events" />
            </ListItemButton>
            <ListItemButton
              selected={activeFilter === "past"}
              onClick={() => {
                setActiveFilter("past");
                setPage(1);
              }}
            >
              <ListItemText primary="Past events" />
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
