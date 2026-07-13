import { Box, CssBaseline, Container } from "@mui/material";
import NavBar from "./NavBar";
import { Outlet, useLocation } from "react-router-dom";

function App() {
  const location = useLocation();
  const isHomePage = location.pathname === "/";

  return (
    <Box sx={{ bgcolor: "#eeeeee", minHeight: "100vh" }}>
      <CssBaseline />
      {!isHomePage && <NavBar />}
      {isHomePage ? (
        <Outlet />
      ) : (
        <Container maxWidth="xl" sx={{ mt: 3 }}>
          <Outlet />
        </Container>
      )}
    </Box>
  );
}

export default App;
