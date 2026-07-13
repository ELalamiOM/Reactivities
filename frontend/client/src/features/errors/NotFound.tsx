import { Button, Paper, Typography } from "@mui/material";
import { Link } from "react-router-dom";

export default function NotFound() {
  return (
    <Paper sx={{ p: 5, textAlign: "center", borderRadius: 3 }}>
      <Typography variant="h3" gutterBottom>
        Page Not Found
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        The page you are looking for does not exist.
      </Typography>
      <Button
        component={Link}
        to="/activities"
        variant="contained"
        size="large"
      >
        Go to Activities
      </Button>
    </Paper>
  );
}
