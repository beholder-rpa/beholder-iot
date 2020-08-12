import React, { useContext } from 'react';
import AppContext from '../../../app/AppContext';
import Box from '@material-ui/core/Box';
import { Button, makeStyles } from '@material-ui/core';
import clsx from 'clsx';
import AppContextPropsType, { BeholderTheme } from '../../../app/AppContextProps';

interface AppFixedFooterProps {
  props?: any
}

const AppFixedFooter: React.FC<AppFixedFooterProps> = (props) => {
  const { footer, footerType } = useContext<AppContextPropsType>(AppContext);

  const useStyles = makeStyles((theme: BeholderTheme) => ({
    footer: {
      position: 'fixed',
      left: 'auto',
      bottom: 0,
      zIndex: 99,
      margin: '0',
      width: '100%',
      backgroundColor: theme.palette.background.paper,
      color: theme.palette.text.primary,
      [theme.breakpoints.up('lg')]: {
        width: 'calc(100vw - 19rem)',
      },
      [theme.breakpoints.up('xl')]: {
        width: 'calc(100vw - 21.6rem)',
      },
      '& .footerContainer': {
        padding: '5px 20px',
        [theme.breakpoints.up('md')]: {
          paddingLeft: 32,
          paddingRight: 32,
        },
        [theme.breakpoints.up('xl')]: {
          padding: '10px 32px',
        },
      },
    },
    btnRoot: {
      paddingLeft: 20,
      paddingRight: 20,
    },
  }));

  const classes = useStyles(props);

  return (
    <>
      {footer && footerType === 'fixed' ? (
        <Box className={clsx(classes.footer, 'footer')}>
          <Box
            className='footerContainer'
            alignItems='center'
            flexDirection='row'
            display='flex'>
            <Box>Copy right @crema 2020</Box>
            <Box ml='auto'>
              <Button className={classes.btnRoot} color='primary'>
                Buy Now
              </Button>
            </Box>
          </Box>
        </Box>
      ) : null}
    </>
  );
};

export default AppFixedFooter;
