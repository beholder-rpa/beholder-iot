import React, { useContext } from 'react';
import AppHeader from './AppHeader';
import AppSidebar from './AppSidebar';
import ContentView from '../../ContentView';
import Hidden from '@material-ui/core/Hidden';
import Box from '@material-ui/core/Box';
import useStyles from './index.style';
import clsx from 'clsx';
import AppContext from '../../../app/AppContext';
import AppFixedFooter from './AppFixedFooter';
import { LayoutType } from '../../../app/AppEnums';
import AppContextPropsType from '../../../app/AppContextProps';

interface StandardLayoutProps {
  props?: any
}

const StandardLayout: React.FC<StandardLayoutProps> = (props) => {
  const classes = useStyles(props);
  const { footer, layoutType, footerType } = useContext<AppContextPropsType>(AppContext);

  return (
    <Box
      className={clsx(
        classes.appMain,
        layoutType === LayoutType.BOXED ? classes.boxedLayout : '',
        {
          appMainFooter: footer && footerType === 'fluid',
          appMainFixedFooter: footer && footerType === 'fixed',
        },
      )}>
      <AppSidebar />

      <Box className={classes.mainContent}>
        <Hidden mdDown>
          <Box className={classes.mainContainer}>
            <AppHeader />
            <ContentView>{props.children}</ContentView>
            <AppFixedFooter />
          </Box>
        </Hidden>

        <Hidden lgUp>
          <Box className={classes.mainContainerFull}>
            <AppHeader />
            <ContentView>{props.children}</ContentView>
            <AppFixedFooter />
          </Box>
        </Hidden>
      </Box>
    </Box>
  );
};

export default StandardLayout;
