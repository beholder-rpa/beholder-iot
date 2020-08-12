import React, { ReactNode } from 'react';
import Box from '@material-ui/core/Box';
import { makeStyles } from '@material-ui/core/styles';
import { grey } from '@material-ui/core/colors';

interface AppsFooterProps {
  children: ReactNode;

  [x: string]: any
}

const AppsFooter: React.FC<AppsFooterProps> = ({
  children, ...rest
}) => {
  const useStyles = makeStyles(() => ({
    paginationRoot: {
      paddingLeft: 8,
      paddingRight: 8,
      borderTop: '1px solid',
      borderColor: grey[300],
    },
    paddingY: {
      paddingTop: 0,
      paddingBottom: 0,
    },
  }));

  const classes = useStyles();
  return (
    <Box className={classes.paginationRoot} {...rest}>
      {children}
    </Box>
  );
};

export default AppsFooter;
