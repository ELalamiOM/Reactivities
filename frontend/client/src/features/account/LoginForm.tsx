import { Box, Button, Paper, Typography, TextField } from "@mui/material";
import { LockOpen } from "@mui/icons-material";
import React, { useState } from "react";
import { z } from "zod";
import { useAccount } from "../../hooks/useAccount";

export default function LoginForm() {
  const { loginUser } = useAccount();

  const loginSchema = z.object({
    email: z.string().min(1, "Email is required").email("Invalid email"),
    password: z.string().min(6, "Password must be at least 6 characters"),
  });

  type LoginSchema = z.infer<typeof loginSchema>;

  const [values, setValues] = useState<LoginSchema>({ email: "", password: "" });
  const [errors, setErrors] = useState<Partial<Record<keyof LoginSchema, string>>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validate = (fieldValues: Partial<LoginSchema> = values) => {
  
      loginSchema.parse(fieldValues);
      setErrors({});
      return true;

  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    const next = { ...values, [name]: value } as LoginSchema;
    setValues(next);
    validate(next);
  };

  const onSubmit = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!validate()) return;
    setIsSubmitting(true);
    try {
      await loginUser.mutateAsync(values);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Paper
  component="form"
  onSubmit={(onSubmit)}
  sx={{
    display: "flex",
    flexDirection: "column",
    p: 3,
    gap: 3,
    maxWidth: "md",
    mx: "auto",
    borderRadius: 3,
  }}
>
 <Box
  sx={{
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    gap: 3,
    color: "secondary.main"
  }}
>
  <LockOpen fontSize="large" />
  <Typography variant="h4">Sign in</Typography>
</Box>

      <TextField
        name="email"
        value={values.email}
        onChange={handleChange}
        label="Email"
        error={!!errors.email}
        helperText={errors.email}
        fullWidth
      />

      <TextField
        name="password"
        value={values.password}
        onChange={handleChange}
        label="Password"
        type="password"
        error={!!errors.password}
        helperText={errors.password}
        fullWidth
      />

      <Button type="submit" disabled={isSubmitting} variant="contained" size="large">
        Login
      </Button>
</Paper>
  )
}