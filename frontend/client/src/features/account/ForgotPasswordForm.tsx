import { LockOpen } from "@mui/icons-material";
import { Box, Button, Paper, TextField, Typography } from "@mui/material";
import React, { useState } from "react";
import { Link } from "react-router-dom";
import { useAccount } from "../../hooks/useAccount";
import {
  forgotPasswordSchema,
  type ForgotPasswordSchema,
} from "../../schemas/forgotPasswordSchema";

export default function ForgotPasswordForm() {
  const { forgotPassword } = useAccount();
  const [values, setValues] = useState<ForgotPasswordSchema>({ email: "" });
  const [errors, setErrors] = useState<
    Partial<Record<keyof ForgotPasswordSchema, string>>
  >({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validate = (fieldValues: Partial<ForgotPasswordSchema> = values) => {
    const result = forgotPasswordSchema.safeParse(fieldValues);

    if (!result.success) {
      const newErrors: Partial<Record<keyof ForgotPasswordSchema, string>> = {};
      result.error.issues.forEach((issue) => {
        if (issue.path.length > 0) {
          newErrors[issue.path[0] as keyof ForgotPasswordSchema] = issue.message;
        }
      });
      setErrors(newErrors);
      return false;
    }

    setErrors({});
    return true;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    const next = { ...values, [name]: value } as ForgotPasswordSchema;
    setValues(next);
    validate(next);
  };

  const onSubmit = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!validate()) return;
    setIsSubmitting(true);
    try {
      await forgotPassword.mutateAsync(values);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Paper
      component="form"
      onSubmit={onSubmit}
      sx={{
        display: "flex",
        flexDirection: "column",
        p: 3,
        gap: 3,
        maxWidth: "lg",
        mx: "auto",
        borderRadius: 3,
      }}
    >
      <Box
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          gap: 2,
          color: "secondary.main",
        }}
      >
        <LockOpen fontSize="large" />
        <Typography variant="h4">Please enter your email address</Typography>
      </Box>

      <TextField
        name="email"
        value={values.email}
        onChange={handleChange}
        label="Email address"
        error={!!errors.email}
        helperText={errors.email}
        fullWidth
      />

      <Button type="submit" disabled={isSubmitting} variant="contained" size="large">
        Request password reset link
      </Button>

      <Typography sx={{ textAlign: "center" }}>
        Back to <Link to="/login">Sign in</Link>
      </Typography>
    </Paper>
  );
}
