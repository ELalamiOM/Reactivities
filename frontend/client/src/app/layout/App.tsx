import { useState, useEffect } from 'react';
import { Typography, List, ListItem } from '@mui/material';
import axios from "axios";
function App() {
  
const [activities, setActivities] = useState<Activity[]>([]);

  useEffect(() => {
    /* fetch('https://localhost:5001/api/activities')
      .then(response => response.json())
      .then(data => setActivities(data)) */
      axios.get<Activity[]>('https://localhost:5001/api/activities')
         .then(response => setActivities(response.data))
  }, []);
  
return (
    <>
      <Typography className="app" style={{ color: 'red' }}>
        Reactivities
      </Typography>
      <List>
        {activities.map((activity) => (
          <ListItem key={activity.id}>
           {activity.title} 
          </ListItem>
        ))}
      </List>
    </>
  );

}

export default App
