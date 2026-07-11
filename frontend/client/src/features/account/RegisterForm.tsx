import { LockOpen } from "@mui/icons-material";
import { Box, Button, Paper, TextField, Typography } from "@mui/material";
import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAccount } from "../../hooks/useAccount";
import {
  registerSchema,
  type RegisterSchema,
} from "../../schemas/registerSchema";

export default function RegisterForm() {
  const navigate = useNavigate();
  const { registerAccount } = useAccount();
  const [values, setValues] = useState<RegisterSchema>({
    email: "",
    displayName: "",
    password: "",
  });
  const [errors, setErrors] = useState<
    Partial<Record<keyof RegisterSchema, string>>
  >({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validate = (fieldValues: Partial<RegisterSchema> = values) => {
    const result = registerSchema.safeParse(fieldValues);

    if (!result.success) {
      const newErrors: Partial<Record<keyof RegisterSchema, string>> = {};
      result.error.issues.forEach((issue) => {
        if (issue.path.length > 0) {
          newErrors[issue.path[0] as keyof RegisterSchema] = issue.message;
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
    const next = { ...values, [name]: value } as RegisterSchema;
    setValues(next);
    validate(next);
  };

  const onSubmit = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!validate()) return;
    setIsSubmitting(true);
    try {
      await registerAccount.mutateAsync(values);
      navigate("/login");
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
          color: "secondary.main",
        }}
      >
        <LockOpen fontSize="large" />
        <Typography variant="h4">Register</Typography>
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
        name="displayName"
        value={values.displayName}
        onChange={handleChange}
        label="Display name"
        error={!!errors.displayName}
        helperText={errors.displayName}
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
        Register
      </Button>

      <Typography variant="h6" sx={{ textAlign: "center" }}>
        Already have an account? <Link to="/login">Sign in</Link>
      </Typography>
    </Paper>
  );
}
