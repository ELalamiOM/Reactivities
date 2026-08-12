import { Chip, type ChipProps } from "@mui/material";
import LocalOfferIcon from "@mui/icons-material/LocalOffer";
import CardGiftcardIcon from "@mui/icons-material/CardGiftcard";
import { formatCredits, isFreeActivity } from "../../../lib/types/credits";

type Props = {
  price: number;
  size?: ChipProps["size"];
};

export default function PriceChip({ price, size = "small" }: Props) {
  const isFree = isFreeActivity(price);

  return (
    <Chip
      size={size}
      label={formatCredits(price)}
      color={isFree ? "success" : "primary"}
      variant={isFree ? "outlined" : "filled"}
      icon={isFree ? <CardGiftcardIcon /> : <LocalOfferIcon />}
      sx={{ borderRadius: 2, fontWeight: 600 }}
    />
  );
}