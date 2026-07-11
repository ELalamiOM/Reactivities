import {
  Box,
  Button,
  MenuItem,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import agent from "../../../api/agent";
import {
  activitySchema,
  type ActivitySchema,
} from "../../../schemas/activitySchema";

const categories = ["Drinks", "Culture", "Film", "Food", "Music", "Travel"];

export default function ActivityForm() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [values, setValues] = useState<ActivitySchema>({
    title: "",
    description: "",
    category: "",
    date: "",
    location: "",
  });
  const [errors, setErrors] = useState<
    Partial<Record<keyof ActivitySchema, string>>
  >({});

  const createActivity = useMutation({
    mutationFn: async (data: ActivitySchema) => {
      const [latitude, longitude] = data.location
        .split(",")
        .map((part) => parseFloat(part.trim()));

      const payload = {
        title: data.title,
        description: data.description,
        category: data.category,
        date: new Date(data.date).toISOString(),
        isCancelled: false,
        city: data.location,
        venue: data.location,
        latitude: Number.isNaN(latitude) ? 0 : latitude,
        longitude: Number.isNaN(longitude) ? 0 : longitude,
      };

      const response = await agent.post<string>("/api/activities", payload);
      return response.data;
    },
    onSuccess: async (id) => {
      await queryClient.invalidateQueries({ queryKey: ["activities"] });
      navigate(`/activities/${id}`);
    },
  });

  const validate = (fieldValues: Partial<ActivitySchema> = values) => {
    const result = activitySchema.safeParse(fieldValues);

    if (!result.success) {
      const newErrors: Partial<Record<keyof ActivitySchema, string>> = {};
      result.error.issues.forEach((issue) => {
        if (issue.path.length > 0) {
          newErrors[issue.path[0] as keyof ActivitySchema] = issue.message;
        }
      });
      setErrors(newErrors);
      return false;
    }

    setErrors({});
    return true;
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setValues((prev) => ({ ...prev, [name]: value }));
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    await createActivity.mutateAsync(values);
  };

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        Create activity
      </Typography>

      <Box
        component="form"
        onSubmit={onSubmit}
        sx={{
          display: "flex",
          flexDirection: "column",
          gap: 3,
        }}
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

        <Box sx={{ display: "flex", justifyContent: "end", gap: 3 }}>
          <Button color="inherit" onClick={() => navigate("/activities")}>
            Cancel
          </Button>
          <Button
            type="submit"
            color="success"
            variant="contained"
            disabled={createActivity.isPending}
          >
            Submit
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}