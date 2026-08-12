import {
  Box,
  Button,
  CircularProgress,
  InputAdornment,
  MenuItem,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import React, { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import agent from "../../../api/agent";
import { activitySchema } from "../../../schemas/activitySchema";
//import type { Activity } from "../../../lib/types"; // ajuste ce chemin

const categories = ["Drinks", "Culture", "Film", "Food", "Music", "Travel"];

// price reste une string dans le formulaire (les inputs HTML renvoient des strings)
type FormValues = {
  title: string;
  description: string;
  category: string;
  date: string;
  location: string;
  price: string;
};

const emptyValues: FormValues = {
  title: "",
  description: "",
  category: "",
  date: "",
  location: "",
  price: "0",
};

// convertit une date ISO en valeur compatible <input type="datetime-local"> (heure locale)
function toDateTimeLocal(value: string | Date) {
  const d = new Date(value);
  const offset = d.getTimezoneOffset() * 60000;
  return new Date(d.getTime() - offset).toISOString().slice(0, 16);
}

export default function ActivityForm() {
  const { id } = useParams();
  const isEditMode = !!id;

  const { data: existingActivity, isPending: isLoadingActivity } = useQuery({
    queryKey: ["activity", id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/api/activities/${id}`);
      return response.data;
    },
    enabled: isEditMode,
  });

  if (isEditMode && isLoadingActivity) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 5 }}>
        <CircularProgress />
      </Box>
    );
  }

  const initialValues: FormValues = existingActivity
    ? {
        title: existingActivity.title,
        description: existingActivity.description,
        category: existingActivity.category,
        date: toDateTimeLocal(existingActivity.date),
        location: existingActivity.venue,
        price: existingActivity.price?.toString() ?? "0",
      }
    : emptyValues;

  // key => remonte le formulaire quand l'activité change : plus besoin de useEffect + setState
  return (
    <ActivityFormFields
      key={existingActivity?.id ?? "new"}
      id={id}
      isEditMode={isEditMode}
      initialValues={initialValues}
    />
  );
}

function ActivityFormFields({
  id,
  isEditMode,
  initialValues,
}: {
  id?: string;
  isEditMode: boolean;
  initialValues: FormValues;
}) {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [values, setValues] = useState<FormValues>(initialValues);
  const [errors, setErrors] = useState<
    Partial<Record<keyof FormValues, string>>
  >({});

  const buildPayload = (data: FormValues) => ({
    title: data.title,
    description: data.description,
    category: data.category,
    date: new Date(data.date).toISOString(),
    isCancelled: false,
    city: data.location,
    venue: data.location,
    latitude: 0,
    longitude: 0,
    price: Number(data.price),
  });

  const createActivity = useMutation({
    mutationFn: async (data: FormValues) => {
      const response = await agent.post<string>(
        "/api/activities",
        buildPayload(data),
      );
      return response.data;
    },
    onSuccess: async (newId) => {
      await queryClient.invalidateQueries({ queryKey: ["activities"] });
      navigate(`/activities/${newId}`);
    },
  });

  const editActivity = useMutation({
    mutationFn: async (data: FormValues) => {
      await agent.put("/api/activities", { id, ...buildPayload(data) });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["activities"] });
      await queryClient.invalidateQueries({ queryKey: ["activity", id] });
      navigate(`/activities/${id}`);
    },
  });

  const validate = (fieldValues: FormValues = values) => {
    // price converti en number pour correspondre au schéma Zod
    const result = activitySchema.safeParse({
      ...fieldValues,
      price: Number(fieldValues.price),
    });

    if (!result.success) {
      const newErrors: Partial<Record<keyof FormValues, string>> = {};
      result.error.issues.forEach((issue) => {
        if (issue.path.length > 0) {
          newErrors[issue.path[0] as keyof FormValues] = issue.message;
        }
      });
      setErrors(newErrors);
      return false;
    }
    setErrors({});
    return true;
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) => {
    const { name, value } = e.target;
    setValues((prev) => ({ ...prev, [name]: value }));
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    if (isEditMode) {
      await editActivity.mutateAsync(values);
    } else {
      await createActivity.mutateAsync(values);
    }
  };

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        {isEditMode ? "Edit Activity" : "Create Activity"}
      </Typography>

      <Box
        component="form"
        onSubmit={onSubmit}
        sx={{ display: "flex", flexDirection: "column", gap: 3 }}
      >
        <TextField
          name="title"
          label="Title"
          value={values.title}
          onChange={handleChange}
          error={!!errors.title}
          helperText={errors.title}
        />

        <TextField
          name="description"
          label="Description"
          multiline
          rows={3}
          value={values.description}
          onChange={handleChange}
          error={!!errors.description}
          helperText={errors.description}
        />

        <Box sx={{ display: "flex", gap: 3 }}>
          <TextField
            name="category"
            label="Category"
            select
            fullWidth
            value={values.category}
            onChange={handleChange}
            error={!!errors.category}
            helperText={errors.category}
          >
            {categories.map((category) => (
              <MenuItem key={category} value={category}>
                {category}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            name="date"
            label="Date"
            type="datetime-local"
            fullWidth
            value={values.date}
            onChange={handleChange}
            error={!!errors.date}
            helperText={errors.date}
            slotProps={{ inputLabel: { shrink: true } }}
          />
        </Box>

        <TextField
          name="location"
          label="Enter the location"
          value={values.location}
          onChange={handleChange}
          error={!!errors.location}
          helperText={errors.location}
        />

        <TextField
          name="price"
          label="Price"
          type="number"
          value={values.price}
          onChange={handleChange}
          error={!!errors.price}
          helperText={errors.price}
          sx={{ width: 220 }}
          slotProps={{
            htmlInput: { min: 0, step: "0.01" },
            input: {
              startAdornment: (
                <InputAdornment position="start">MAD</InputAdornment>
              ),
            },
          }}
        />

        <Box sx={{ display: "flex", justifyContent: "end", gap: 3 }}>
          <Button color="inherit" onClick={() => navigate(-1)}>
            Cancel
          </Button>
          <Button
            type="submit"
            color="success"
            variant="contained"
            disabled={createActivity.isPending || editActivity.isPending}
          >
            {isEditMode ? "Save" : "Submit"}
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}