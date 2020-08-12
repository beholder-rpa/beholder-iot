import React from 'react';
import PerfectScrollbar from 'react-perfect-scrollbar';
import Drawer from '@material-ui/core/Drawer';
import Hidden from '@material-ui/core/Hidden';
import clsx from 'clsx';
import Navigation from '../../Navigation/VerticleNav';
import Box from '@material-ui/core/Box';
import useStyles from './AppSidebar.style';

interface AppSidebarProps {
  position?: 'left' | 'bottom' | 'right' | 'top' | undefined
}

const AppSidebar: React.FC<AppSidebarProps> = ({ position = 'left' }) => {
  //const {navCollapsed} = useSelector<AppState, AppState['settings']>(({settings}) => settings);

  const handleToggleDrawer = () => {
    //dispatch(toggleNavCollapsed());
  };
  const classes = useStyles();
  let sidebarClasses = classes.sidebarStandard;
  return (
    <>
      <Hidden lgUp>
        <Drawer
          anchor={position}
          open={true /*navCollapsed*/}
          onClose={() => handleToggleDrawer()}
          style={{ position: 'absolute' }}>
          <Box height='100%' className={classes.container}>
            <Box className={clsx(classes.sidebarBg, sidebarClasses)}>
              <PerfectScrollbar className={classes.drawerScrollAppSidebar}>
                <Navigation />
              </PerfectScrollbar>
            </Box>
          </Box>
        </Drawer>
      </Hidden>
      <Hidden mdDown>
        <Box height='100%' className={clsx(classes.container, 'app-sidebar')}>
          <Box className={clsx(classes.sidebarBg, sidebarClasses)}>
            <PerfectScrollbar className={classes.scrollAppSidebar}>
              <Navigation />
            </PerfectScrollbar>
          </Box>
        </Box>
      </Hidden>
    </>
  );
};

export default AppSidebar;
