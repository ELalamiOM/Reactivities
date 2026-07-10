import { Button,Card, CardActions, CardContent, CardMedia, Typography } from "@mui/material"
import { Link } from 'react-router-dom'
import { useNavigate } from 'react-router-dom';


type Props = {
  activity: Activity
  cancelSelectActivity:() => void
  openForm: (id: string) =>void
}


export default function ActivityDetail({ activity }: Props) {
  const navigate = useNavigate();
  
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
         <Button component={Link} to={`/manage/${activity.id}`} color="primary">
           Edit
         </Button>
         <Button onClick={() => navigate('/activities')} color="inherit">
           Cancel
         </Button>
      </CardActions>
    </Card>
  )
}