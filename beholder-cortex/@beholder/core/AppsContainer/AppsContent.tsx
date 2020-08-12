import React, { ReactNode } from 'react';
import Scrollbar from '../Scrollbar';
import makeStyles from '@material-ui/core/styles/makeStyles';
import { BeholderTheme } from '../../app/AppContextProps';

interface AppsContentProps {
  isDetailView: boolean;
  fullView: boolean;
  children: ReactNode;
}


const AppsContent: React.FC<AppsContentProps> = ({ isDetailView = false, fullView, children }) => {
  // @ts-ignore
  const useStyles = makeStyles((theme: BeholderTheme) => ({
    appsContentContainer: () => ({
      height: `calc(100% - ${isDetailView ? 60 : 115}px)`,
      [theme.breakpoints.up('sm')]: {
        height: `calc(100% - ${fullView ? 0 : 60}px)`,
      },
      [theme.breakpoints.up('xl')]: {
        height: `calc(100% - ${fullView ? 0 : 77}px)`,
      },
      '& .scrum-absolute': {
        position: 'absolute',
        top: 0,
        left: 0,
        width: '100%',
      },
      '& .scrum-row': {
        display: 'inline-flex',
        minWidth: '100%',
        height: '100%',
        marginLeft: '-10px',
        marginRight: '-10px',
      },
      '& .scrum-col': {
        width: '280px',
        marginLeft: '10px',
        marginRight: '10px',
        borderRadius: theme.overrides.MuiCard.root.borderRadius,
        height: '100% !important',
        [theme.breakpoints.up('md')]: {
          width: '354px',
        },
      },
      '& .scroll-scrum-item': {
        height: 'auto !important',
      },
    }),
  }));

  const classes = useStyles();
  return (
    <Scrollbar className={classes.appsContentContainer}>
      {children}
    </Scrollbar>
  );
};

export default AppsContent;
