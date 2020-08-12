import React, { ReactNode } from 'react';
import Hidden from '@material-ui/core/Hidden';
import Drawer from '@material-ui/core/Drawer';
import Card from '@material-ui/core/Card';
import { Box } from '@material-ui/core';
import useStyles from './index.style';
import { NavStyle } from '../../app/AppEnums';

interface AppSidebarProps {
  isAppDrawerOpen: boolean;
  footer: boolean;
  fullView: boolean;
  navStyle: NavStyle;
  sidebarContent: ReactNode;
}


const AppSidebar: React.FC<AppSidebarProps> = (props) => {
  const { isAppDrawerOpen, footer, navStyle, fullView, sidebarContent } = props;
  const classes = useStyles({ footer, navStyle, fullView });
  return (
    <Box className={classes.appsSidebar}>
      <Hidden lgUp>
        <Drawer
          open={isAppDrawerOpen}
          onClose={() => { } /*dispatch(onToggleAppDrawer())*/}
          style={{ position: 'absolute' }}>
          {sidebarContent}
        </Drawer>
      </Hidden>
      <Hidden mdDown>
        <Card style={{ height: '100%' }}>{sidebarContent}</Card>
      </Hidden>
    </Box>
  );
};

export default AppSidebar;
