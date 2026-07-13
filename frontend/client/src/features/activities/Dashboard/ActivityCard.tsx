import { AccessTime } from "@mui/icons-material";
import {
  Avatar,
  AvatarGroup,
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Chip,
  Divider,
  Typography,
} from "@mui/material";
import Place from "@mui/icons-material/Place";
import { Link } from "react-router-dom";
import { useAccount } from "../../../hooks/useAccount";

type Props = {
  activity: Activity;
};

export default function ActivityCard({ activity }: Props) {
  const { currentUser } = useAccount();

  const isHost = currentUser?.id === activity.hostId;
  const isGoing =
    activity.attendees?.some((a) => a.id === currentUser?.id) ?? false;
  const label = isHost ? "You are hosting" : "You are going";
  const color = isHost ? "secondary" : isGoing ? "warning" : "default";
  const host = activity.attendees?.find((a) => a.id === activity.hostId);

  return (
    <Card elevation={3} sx={{ borderRadius: 3 }}>
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
        }}
      >
        <CardHeader
          avatar={
            <Avatar src={host?.imageUrl} sx={{ height: 80, width: 80 }} />
          }
          title={activity.title}
          titleTypographyProps={{ fontWeight: "bold", fontSize: 20 }}
          subheader={
            <>
              Hosted by <Link to={`/profile`}>{activity.hostDisplayName}</Link>
            </>
          }
        />
        <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mr: 2 }}>
          {(isHost || isGoing) && (
            <Chip label={label} color={color} sx={{ borderRadius: 2 }} />
          )}
          {activity.isCancelled && (
            <Chip label="Cancelled" color="error" sx={{ borderRadius: 2 }} />
          )}
        </Box>
      </Box>

      <Divider sx={{ mb: 3 }} />

      <CardContent sx={{ p: 0 }}>
        <Box sx={{ display: "flex", alignItems: "center", mb: 2, px: 2 }}>
          <AccessTime sx={{ mr: 1 }} />
          <Typography variant="body2">
            {new Date(activity.date).toLocaleDateString()}
          </Typography>
          <Place sx={{ ml: 3, mr: 1 }} />
          <Typography variant="body2">{activity.venue}</Typography>
        </Box>
        <Divider />
      </CardContent>

      <Box
        sx={{
          display: "flex",
          gap: 2,
          backgroundColor: "grey.200",
          py: 2,
          pl: 3,
          alignItems: "center",
        }}
      >
        <AvatarGroup max={5}>
          {activity.attendees?.map((attendee) => (
            <Avatar
              key={attendee.id}
              src={attendee.imageUrl}
              sx={{ width: 32, height: 32 }}
            >
              {attendee.displayName?.charAt(0)}
            </Avatar>
          ))}
        </AvatarGroup>
      </Box>

      <CardActions sx={{ pb: 2, justifyContent: "space-between" }}>
        <Typography variant="body2" sx={{ pl: 1, flex: 1 }}>
          {activity.description}
        </Typography>
        <Button
          component={Link}
          to={`/activities/${activity.id}`}
          size="medium"
          variant="contained"
          sx={{ borderRadius: 3 }}
        >
          View
        </Button>
      </CardActions>
    </Card>
  );
}
