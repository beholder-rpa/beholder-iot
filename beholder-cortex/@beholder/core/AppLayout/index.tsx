import React, { ReactNode, useContext } from 'react';
import { makeStyles } from '@material-ui/core/styles';

import AppContext from '../../app/AppContext';
import Layouts from './Layouts';
import useStyles from '../../app/common.style';
import AppContextProps from '../../app/AppContextProps';

const useStyle = makeStyles(() => ({
  appAuth: {
    flex: 1,
    display: 'flex',
    position: 'relative',
    height: '100vh',
    backgroundColor: '#f3f4f6',
    background: `url(/images/auth-background.jpg) no-repeat center center`,
    backgroundSize: 'cover',

    '& .scrollbar-container': {
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
    },
    '& .main-content-view': {
      padding: 20,
    },
    '& .footer': {
      marginRight: 0,
      marginLeft: 0,
    },
  },
}));

interface CremaLayoutProps {
  children: ReactNode;
}

const CremaLayout: React.FC<CremaLayoutProps> = ({ children }) => {
  useStyles();
  const { navStyle } = useContext<AppContextProps>(AppContext);
  const AppLayout = Layouts[navStyle];

  const classes = useStyle();
  return (
    <AppLayout>{children}</AppLayout>
  );
};

export default React.memo(CremaLayout);
