import {
  Box,
  Button,
  CircularProgress,
  MenuItem,
  Paper,
  TextField,
  Typography,
} from "@mui/material";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import agent from "../../../api/agent";
import {
  activitySchema,
  type ActivitySchema,
} from "../../../schemas/activitySchema";

const categories = ["Drinks", "Culture", "Film", "Food", "Music", "Travel"];

export default function ActivityForm() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { id } = useParams();
  const isEditMode = !!id;

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

  const { data: existingActivity, isPending: isLoadingActivity } = useQuery({
    queryKey: ["activity", id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/api/activities/${id}`);
      return response.data;
    },
    enabled: isEditMode,
  });

  useEffect(() => {
    if (existingActivity) {
      setValues({
        title: existingActivity.title,
        description: existingActivity.description,
        category: existingActivity.category,
        date: new Date(existingActivity.date).toISOString().slice(0, 16),
        location: existingActivity.venue,
      });
    }
  }, [existingActivity]);

  const createActivity = useMutation({
    mutationFn: async (data: ActivitySchema) => {
      const payload = buildPayload(data);
      const response = await agent.post<string>("/api/activities", payload);
      return response.data;
    },
    onSuccess: async (newId) => {
      await queryClient.invalidateQueries({ queryKey: ["activities"] });
      navigate(`/activities/${newId}`);
    },
  });

  const editActivity = useMutation({
    mutationFn: async (data: ActivitySchema) => {
      const payload = { id, ...buildPayload(data) };
      await agent.put("/api/activities", payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["activities"] });
      await queryClient.invalidateQueries({ queryKey: ["activity", id] });
      navigate(`/activities/${id}`);
    },
  });

  const buildPayload = (data: ActivitySchema) => ({
    title: data.title,
    description: data.description,
    category: data.category,
    date: new Date(data.date).toISOString(),
    isCancelled: false,
    city: data.location,
    venue: data.location,
    latitude: 0,
    longitude: 0,
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

  if (isEditMode && isLoadingActivity) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 5 }}>
        <CircularProgress />
      </Box>
    );
  }

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
