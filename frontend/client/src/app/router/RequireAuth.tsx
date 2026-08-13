import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Box, CircularProgress } from "@mui/material";
import { useAccount } from "../../hooks/useAccount";

export default function RequireAuth() {
  const { currentUser, loadingUserInfo } = useAccount();
  const location = useLocation();

  if (loadingUserInfo) {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100vh",
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (!currentUser) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
}
