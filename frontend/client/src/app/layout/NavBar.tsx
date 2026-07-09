import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import GroupIcon from '@mui/icons-material/Group';
import { Button, MenuItem ,MenuList} from '@mui/material';

type Props = {
    openForm: () => void;
}

export default function NavBar({openForm}: Props) {
  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position="static"
        sx={{
          backgroundImage:
            'linear-gradient(135deg, #182a73 0%, #218aae 69%, #20a7ac 89%)',
        }}
      >
        <Container maxWidth="xl">
          <Toolbar
            sx={{
              display: 'flex',
              justifyContent: 'space-between',
              minHeight: '80px',
            }}
          >
            {/* Logo */}
            <Box>
                <MenuList sx={{ p: 0 }}>
                <MenuItem sx={{ display: 'flex', gap: 2 }}>
              <GroupIcon fontSize="large" />

              <Typography
                variant="h4"
                sx={{ fontWeight: 'bold' , fontSize: '2rem'}}
              >
                Reactivities
              </Typography>
               </MenuItem>
               </MenuList>
            </Box>

            {/* Menu */}
           <Box>
  <MenuList
    sx={{
      display: 'flex',
      flexDirection: 'row',
      p: 0
    }}
  >
    <MenuItem
      sx={{
        fontSize: '1.2rem',
        textTransform: 'uppercase',
        fontWeight: 'bold',
      }}
    >
      Activities
    </MenuItem>

    <MenuItem
      sx={{
        fontSize: '1.2rem',
        textTransform: 'uppercase',
        fontWeight: 'bold',
      }}
    >
      About
    </MenuItem>

    <MenuItem
      sx={{
        fontSize: '1.2rem',
        textTransform: 'uppercase',
        fontWeight: 'bold',
      }}
    >
      Contact
    </MenuItem>
  </MenuList>
</Box>
         <Button size="large" variant="contained" color="warning" onClick={openForm}>Create activity</Button>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}