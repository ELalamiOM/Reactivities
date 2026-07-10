import { Box, Button, Paper, TextField, Typography } from '@mui/material';
// import type { FormEvent } from "react";
// import { useActivities } from "../../../hooks/useActivities";
// import { useNavigate } from 'react-router-dom';

export default function ActivityForm() {

 // const { updateActivity, createActivity, activity } = useActivities();
 // const navigate = useNavigate();
  /*
    const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const formData = new FormData(event.currentTarget);

        const data: { [key: string]: FormDataEntryValue } = {}
        formData.forEach((value, key) => {
            data[key] = value;
        });

        if (activity) {
            data.id = activity.id;
            await updateActivity.mutateAsync(data as unknown as Activity);
            navigate(`/activities/${activity.id}`);
          } else {
            await createActivity.mutateAsync(data as unknown as Activity);

        }
      }
        */
  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant="h5" gutterBottom color="primary">
        Create activity
      </Typography>

      <Box  component="form"
    sx={{
    display: 'flex',
    flexDirection: 'column',
    gap: 3,
  }}
>
        <TextField label="Title" />
        <TextField label="Description" multiline rows={3} />
        <TextField label="Category" />
        <TextField label="Date" type="date" />
        <TextField label="City" />
        <TextField label="Venue" />

        <Box sx ={{ display:"flex"  ,justifyContent:"end", gap:3 }} >
          <Button color="inherit">Cancel</Button>
          <Button color="success" variant="contained">
            Submit
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}