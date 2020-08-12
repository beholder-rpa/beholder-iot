import React, { ReactNode, useContext } from 'react';
import InfoView from '../InfoView';
import { Box } from '@material-ui/core';
import Hidden from '@material-ui/core/Hidden';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import { Fonts } from '../../app/AppEnums';
import Card from '@material-ui/core/Card';
import useStyles from './index.style';
import AppContext from '../../app/AppContext';
import AppSidebar from './AppSidebar';
import AppContextPropsType from '../../app/AppContextProps';

interface AppsContainerProps {
  title: string;
  sidebarContent: ReactNode;
  fullView: boolean;
  cardStyle: any;
  children: ReactNode
}

const AppsContainer: React.FC<AppsContainerProps> = ({ title = '', cardStyle, sidebarContent, fullView, children }) => {
  //const dispatch = useDispatch();
  //const { isAppDrawerOpen } = useSelector<AppState, AppState['common']>(({ common }) => common);
  const { footer, navStyle } = useContext<AppContextPropsType>(AppContext);
  const classes = useStyles({ footer, navStyle, fullView });

  const isAppDrawerOpen = false;
  
  return (
    <Box
      pt={{ xl: 4 }}
      flex={1}
      display='flex'
      className='XXXXX'
      flexDirection='column'>
      <Box
        mb={{ xs: 2, lg: 4 }}
        mt={{ xs: -3, lg: 0 }}
        display='flex'
        alignItems='center'>
        <Hidden lgUp>
          <IconButton
            edge='start'
            className={classes.menuButton}
            color='inherit'
            aria-label='open drawer'
            onClick={() => { }/*dispatch(onToggleAppDrawer())*/}>
            <MenuIcon className={classes.menuIcon} />
          </IconButton>
        </Hidden>
        <Box
          component='h2'
          color='text.primary'
          fontFamily={Fonts.LIGHT}
          fontSize={{ xs: 18, sm: 20, xl: 22 }}>
          {title}
        </Box>
      </Box>

      <Box className={classes.appsContainer}>
        {sidebarContent ? (
          <AppSidebar
            isAppDrawerOpen={isAppDrawerOpen}
            footer={footer}
            fullView={fullView}
            navStyle={navStyle}
            sidebarContent={sidebarContent}
          />
        ) : null}

        <Box className={classes.appsMainContent}>
          <Card style={{ height: '100%', ...cardStyle }}>{children}</Card>
          <InfoView />
        </Box>
      </Box>
    </Box>
  );
};

export default AppsContainer;
