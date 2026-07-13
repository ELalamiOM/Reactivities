import { Button, Paper, Typography } from "@mui/material";
import { Link } from "react-router-dom";

export default function ServerError() {
  return (
    <Paper sx={{ p: 5, textAlign: "center", borderRadius: 3 }}>
      <Typography variant="h3" gutterBottom color="error">
        Server Error
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        An internal server error has occurred. Please try again later.
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
