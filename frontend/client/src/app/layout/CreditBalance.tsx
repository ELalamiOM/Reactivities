import { Chip, Tooltip } from "@mui/material";
import AccountBalanceWalletIcon from "@mui/icons-material/AccountBalanceWallet";
import { useCredits } from "../../hooks/useCredits";

export default function CreditBalance() {
  const { balance, isLoadingCredits } = useCredits();

  if (isLoadingCredits) return null;

  return (
    <Tooltip title="Votre solde de crédits">
      <Chip
        icon={<AccountBalanceWalletIcon sx={{ color: "#fff !important" }} />}
        label={`${balance} crédits`}
        sx={{
          bgcolor: "rgba(255,255,255,.18)",
          color: "#fff",
          fontWeight: 700,
          borderRadius: 2,
          mr: 2,
        }}
      />
    </Tooltip>
  );
}