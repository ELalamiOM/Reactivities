import { Group } from "@mui/icons-material";
import { Paper, Typography, Box, Button } from "@mui/material";
import { Link } from "react-router-dom";

export default function HomePage() {
  return (
    <Paper
      elevation={0}
      sx={{
        color: "white",
        display: "flex",
        flexDirection: "column",
        gap: 6,
        alignItems: "center",
        justifyContent: "center",
        height: "100vh",
        width: "100vw",
        position: "fixed",
        top: 0,
        left: 0,
        borderRadius: 0,
        backgroundImage:
          "linear-gradient(135deg, #182a73 0%, #218aae 69%, #20a7ac 89%)",
      }}
    >
      <Box
        sx={{ display: "flex", alignItems: "center", color: "white", gap: 3 }}
      >
        <Group sx={{ height: 110, width: 110 }} />
        <Typography variant="h1" sx={{ fontWeight: 400, fontFamily: "Roboto, Helvetica, Arial, sans-serif" }}>Reactivity</Typography>
      </Box>

      <Typography variant="h2" sx={{ fontWeight: 300, fontFamily: "Roboto, Helvetica, Arial, sans-serif" }}>Welcome to reactivity</Typography>

      <Button
        component={Link}
        to="/activities"
        size="large"
        variant="contained"
        sx={{ borderRadius: 4, fontSize: "1.5rem" }}
      >
        Take me to the activities!
      </Button>
    </Paper>
  );
}
