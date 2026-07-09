import { Card, CardActions, CardContent, CardMedia, Typography } from "@mui/material"

type Props = {
  activity: Activity
  cancelSelectActivity:() => void
  openForm: (id: string) =>void
}

export default function ActivityDetail({ activity,cancelSelectActivity }: Props) {
  return (
    <Card sx={{ borderRadius: 3 }}>
      <CardMedia
        component='img'
        src={"/images/categoryImages/${activity.category}.png"} />
        <CardContent>
        <Typography variant="h5">{activity.title}</Typography>
         <Typography variant="subtitle1" sx={{ fontWeight: 300 }}>
          {activity.date}
        </Typography>
        <Typography variant="body1">
          {activity.description}
        </Typography>
      </CardContent>
      <CardActions>
         <button color="primary">Edit</button>
         <button onClick={cancelSelectActivity} color="inherit">Cancel</button>
      </CardActions>
    </Card>
  )
}