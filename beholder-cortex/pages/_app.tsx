import React from 'react';
import ContextProvider from '../@beholder/app/ContextProvider';
import BeholderThemeProvider from '../@beholder/app/BeholderThemeProvider';
import BeholderStyleProvider from '../@beholder/app/BeholderStyleProvider';
import CssBaseline from '@material-ui/core/CssBaseline';
import { NextComponentType } from 'next';
import { AppContext, AppInitialProps, AppProps } from 'next/app';

import 'react-perfect-scrollbar/dist/css/styles.css';
import '../styles/base.css';

const BeholderApp: NextComponentType<AppContext, AppInitialProps, AppProps> = ({ Component, pageProps }) => {

  React.useEffect(() => {
    // Remove the server-side injected CSS.
    const jssStyles = document.querySelector('#jss-server-side');
    if (jssStyles) {
      jssStyles.parentElement!.removeChild(jssStyles);
    }
  }, []);

  return (
    <ContextProvider>
      <BeholderThemeProvider>
        <BeholderStyleProvider>
            <CssBaseline />
            <Component {...pageProps} />
        </BeholderStyleProvider>
      </BeholderThemeProvider>
    </ContextProvider>
  );
};
export default BeholderApp;

BeholderApp.getInitialProps = async ({ Component, ctx }: AppContext) => {
  const pageProps = {
    ...(Component.getInitialProps ? await Component.getInitialProps(ctx) : {}),
  };
  return { pageProps };
};

