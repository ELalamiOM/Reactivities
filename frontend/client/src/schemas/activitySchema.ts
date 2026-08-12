import { z } from "zod";

export const activitySchema = z.object({
  title: z.string().min(1, "Title is required"),
  description: z.string().min(1, "Description is required"),
  category: z.string().min(1, "Category is required"),
  date: z.string().min(1, "Date is required"),
  location: z.string().min(1, "Location is required"),
  price: z.coerce
    .number({ error: "Price must be a number" })
    .min(0, "Price cannot be negative")
    .max(1_000_000, "Price is too high"),
});

export type ActivitySchema = z.infer<typeof activitySchema>;
