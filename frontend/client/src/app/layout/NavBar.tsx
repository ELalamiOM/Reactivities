import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Container from "@mui/material/Container";
import Typography from "@mui/material/Typography";
import GroupIcon from "@mui/icons-material/Group";
import { Button } from "@mui/material";
import { NavLink } from "react-router-dom";
import { useAccount } from "../../hooks/useAccount";
import UserMenu from "./UserMenu";

export default function NavBar() {
  const { currentUser } = useAccount();

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position="static"
        sx={{
          backgroundImage:
            "linear-gradient(135deg, #182a73 0%, #218aae 69%, #20a7ac 89%)",
        }}
      >
        <Container maxWidth="xl">
          <Toolbar
            sx={{
              display: "flex",
              justifyContent: "space-between",
              minHeight: "80px",
            }}
          >
            <Box>
              <Button
                component={NavLink}
                to="/"
                sx={{ display: "flex", gap: 2, color: "#eeeeee" }}
              >
                <GroupIcon fontSize="large" />
                <Typography
                  variant="h4"
                  sx={{ fontWeight: "bold", fontSize: "2rem" }}
                >
                  Reactivities
                </Typography>
              </Button>
            </Box>

            <Box>
              <Button
                component={NavLink}
                to="/activities"
                sx={{
                  fontSize: "1.2rem",
                  textTransform: "uppercase",
                  fontWeight: "bold",
                  color: "#eeeeee",
                }}
              >
                Activities
              </Button>
              {currentUser && (
                <Button
                  component={NavLink}
                  to="/createActivity"
                  sx={{
                    fontSize: "1.2rem",
                    textTransform: "uppercase",
                    fontWeight: "bold",
                    color: "#eeeeee",
                  }}
                >
                  Create Activity
                </Button>
              )}
            </Box>

            <Box sx={{ display: "flex", alignItems: "center" }}>
              {currentUser ? (
                <UserMenu />
              ) : (
                <>
                  <Button
                    component={NavLink}
                    to="/login"
                    sx={{ color: "#eeeeee" }}
                  >
                    Login
                  </Button>
                  <Button
                    component={NavLink}
                    to="/register"
                    sx={{ color: "#eeeeee" }}
                  >
                    Register
                  </Button>
                </>
              )}
            </Box>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}
