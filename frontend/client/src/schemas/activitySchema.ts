import { z } from "zod";

export const activitySchema = z.object({
  title: z.string().min(1, "Title is required"),
  description: z.string().min(1, "Description is required"),
  category: z.string().min(1, "Category is required"),
  date: z.string().min(1, "Date is required"),
  location: z.string().min(1, "Location is required"),
});

export type ActivitySchema = z.infer<typeof activitySchema>;
