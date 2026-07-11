import { z } from "zod";

export const registerSchema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email"),
  displayName: z.string().min(1, "Display name is required"),
  password: z.string().min(6, "Password must be at least 6 characters"),
});

export type RegisterSchema = z.infer<typeof registerSchema>;
