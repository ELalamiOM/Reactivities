import { Alert, Box, Button, CircularProgress, Snackbar, Tooltip, Typography } from "@mui/material";
import { useState } from "react";
import { useAccount } from "../../hooks/useAccount";
import { useCredits } from "../../hooks/useCredits";
import { useInscriptions } from "../../hooks/useInscriptions";
import { formatCredits, isFreeActivity } from "../../lib/types/credits";

type Props = { activity: Activity };

export default function JoinActivityButton({ activity }: Props) {
  const { currentUser } = useAccount();
  const { balance, isLoadingCredits } = useCredits();
  const { register, unregister, getApiError } = useInscriptions(activity.id);
  const [error, setError] = useState<string | null>(null);

  const isHost = currentUser?.id === activity.hostId;
  const isGoing = activity.attendees?.some((a) => a.id === currentUser?.id) ?? false;

  const price = activity.price ?? 0;
  const isFree = isFreeActivity(price);
  const hasEnoughCredits = isFree || balance >= price;
  const isPending = register.isPending || unregister.isPending;

  // L'hôte ne peut pas se désinscrire (règle backend, 403)
  if (isHost) {
    return (
      <Typography variant="body2" color="text.secondary">
        Vous êtes l'organisateur de cette activité.
      </Typography>
    );
  }

  const handleClick = async () => {
    setError(null);
    try {
      if (isGoing) {
        await unregister.mutateAsync(activity.id);
      } else {
        await register.mutateAsync(activity.id);
      }
    } catch (e) {
      setError(getApiError(e, "Une erreur est survenue. Veuillez réessayer."));
    }
  };

  const disabled =
    isPending || isLoadingCredits || activity.isCancelled || (!isGoing && !hasEnoughCredits);

  const tooltip = activity.isCancelled
    ? "Cette activité est annulée"
    : !isGoing && !hasEnoughCredits
      ? `Solde insuffisant : ${balance} / ${price} crédits`
      : "";

  return (
    <Box sx={{ display: "flex", flexDirection: "column", alignItems: "flex-end", gap: 1 }}>
      <Tooltip title={tooltip} disableHoverListener={!tooltip}>
        <span>
          <Button
            variant="contained"
            color={isGoing ? "error" : "success"}
            onClick={handleClick}
            disabled={disabled}
            sx={{ borderRadius: 3, minWidth: 180 }}
            startIcon={isPending ? <CircularProgress size={16} color="inherit" /> : undefined}
          >
            {isGoing
              ? isFree ? "Se désinscrire" : `Se désinscrire (+${price} crédits)`
              : isFree ? "S'inscrire (Gratuit)" : `S'inscrire (${formatCredits(price)})`}
          </Button>
        </span>
      </Tooltip>

      {!isGoing && !isFree && (
        <Typography
          variant="caption"
          color={hasEnoughCredits ? "text.secondary" : "error"}
        >
          {hasEnoughCredits
            ? `Solde : ${balance} crédits → ${balance - price} après inscription`
            : `Solde insuffisant : ${balance} crédits (il en faut ${price})`}
        </Typography>
      )}

      <Snackbar
        open={!!error}
        autoHideDuration={5000}
        onClose={() => setError(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert severity="error" onClose={() => setError(null)}>
          {error}
        </Alert>
      </Snackbar>
    </Box>
  );
}