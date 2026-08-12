export function formatCredits(price: number) {
  if (!price || price <= 0) return "Gratuit";
  return `${price} ${price === 1 ? "crédit" : "crédits"}`;
}

export function isFreeActivity(price: number) {
  return !price || price <= 0;
}