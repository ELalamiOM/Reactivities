import { useState } from 'react';
import { Avatar, Box, Divider, ListItemIcon, Menu, MenuItem, Typography } from '@mui/material';
import AddCircleOutlinedIcon from '@mui/icons-material/AddCircleOutlined';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import LogoutIcon from '@mui/icons-material/Logout';
import { useNavigate } from 'react-router-dom';
import { useAccount } from '../../hooks/useAccount';

export default function UserMenu() {
  const { currentUser, logoutUser } = useAccount();
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = async () => {
    handleClose();
    await logoutUser.mutateAsync();
    navigate('/login');
  };

  return (
    <>
      <Box
        onClick={handleOpen}
        sx={{ display: 'flex', alignItems: 'center', gap: 1, cursor: 'pointer' }}
      >
        <Avatar sx={{ bgcolor: 'rgba(255,255,255,0.3)', color: '#fff', width: 40, height: 40 }}>
          {currentUser?.displayName?.charAt(0).toUpperCase() ?? '?'}
        </Avatar>
        <Typography sx={{ color: '#fff', fontWeight: 'bold', fontSize: '1rem' }}>
          {currentUser?.displayName?.toUpperCase()}
        </Typography>
      </Box>

      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        slotProps={{
          paper: {
            sx: {
              mt: 1.5,
              minWidth: 200,
              borderRadius: 2,
              boxShadow: 4,
            },
          },
        }}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
      >
        <MenuItem
          onClick={() => { handleClose(); navigate('/createActivity'); }}
          sx={{ py: 1.5 }}
        >
          <ListItemIcon>
            <AddCircleOutlinedIcon fontSize="small" />
          </ListItemIcon>
          Create Activity
        </MenuItem>

        <MenuItem
          onClick={() => { handleClose(); navigate('/profile'); }}
          sx={{ py: 1.5 }}
        >
          <ListItemIcon>
            <AccountCircleIcon fontSize="small" />
          </ListItemIcon>
          My profile
        </MenuItem>

        <Divider />

        <MenuItem onClick={handleLogout} sx={{ py: 1.5 }}>
          <ListItemIcon>
            <LogoutIcon fontSize="small" />
          </ListItemIcon>
          Logout
        </MenuItem>
      </Menu>
    </>
  );
}
