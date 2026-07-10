import { Group } from "@mui/icons-material";
import { Paper, Typography, Box, Button } from "@mui/material";
import { createBrowserRouter, Link } from "react-router";
import ActivitiesDashboard from "../../features/activities/Dashboard/ActivityDashboard";
import ActivitiesForm from "../../features/activities/form/ActivityForm";
import App from "../../app/layout/App";

// eslint-disable-next-line react-refresh/only-export-components
export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            { path: "", element: <HomePage /> }, 
            { path: "activities", element: <ActivitiesDashboard /> },
            { path: "createActivity", element: <ActivitiesForm /> }
          ]
    }
]);

export default function HomePage() {
  return (
    <Paper
      sx={{
        color: 'white',
        display: 'flex',
        flexDirection: 'column',
        gap: 6,
        alignItems: 'center',
        alignContent: 'center',
        justifyContent: 'center',
        height: '100vh',
        backgroundImage:
          'linear-gradient(135deg, #182a73 0%, #218aae 69%, #20a7ac 89%)'
      }}
    >

        <Box
  sx={{
    display: 'flex',
    alignItems: 'center',
    alignContent: 'center',
    color: 'white',
    gap: 3
  }}
>
  <Group sx={{ height: 110, width: 110 }} />

  <Typography variant="h1">
    Reactivities
  </Typography>
</Box>

<Typography variant="h2">
  Welcome to reactivities
</Typography>

<Button
  component={Link}
  to='/activities'
  size="large"
  variant="contained"
  sx={{Height: 80, borderRadius: 4, fontSize:- '1.5rem'}}
>
  Take me to the activities!
</Button>
    </Paper>
  );
}