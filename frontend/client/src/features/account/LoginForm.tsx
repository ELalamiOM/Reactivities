import { Box, Button, Paper, Typography, TextField } from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import { LockOpen } from "@mui/icons-material";
import React, { useState } from "react";
import { useAccount } from "../../hooks/useAccount";
import { useNavigate } from "react-router-dom";
import { loginSchema, type LoginSchema } from "../../schemas/loginSchema";

export default function LoginForm() {
  const { loginUser } = useAccount();
  const navigate = useNavigate();

  const [values, setValues] = useState<LoginSchema>({
    email: "",
    password: "",
  });
  const [errors, setErrors] = useState<
    Partial<Record<keyof LoginSchema, string>>
  >({});

  const validate = (fieldValues: Partial<LoginSchema> = values) => {
    const result = loginSchema.safeParse(fieldValues);

    if (!result.success) {
      const newErrors: Partial<Record<keyof LoginSchema, string>> = {};
      result.error.issues.forEach((err) => {
        if (err.path.length > 0) {
          newErrors[err.path[0] as keyof LoginSchema] = err.message;
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
    const next = { ...values, [name]: value } as LoginSchema;
    setValues(next);
    validate(next);
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    await loginUser.mutateAsync(values);
    navigate("/activities");
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

      <Button
        type="submit"
        disabled={loginUser.isPending}
        variant="contained"
        size="large"
      >
        Login
      </Button>
       <Typography sx={{ mt: 2, display: "flex", justifyContent: "center" }} variant="body2">
         Mot de passe oublié ?  <RouterLink to="/forgot-password"> Ici </RouterLink> &nbsp;&nbsp;
         Pas encore de compte ?  <RouterLink to="/register"> Créer un compte </RouterLink>
        </Typography>
    </Paper>
  );
}
