import { useEffect, useRef } from 'react';
import { observer } from 'mobx-react';
import { QueryClient, QueryClientProvider } from 'react-query';

import { useAppStore, AppStoreContext } from '@stores/AppStore';
import { BeholderClient, BeholderClientContext } from '@services/BeholderClient';
import { BeholderRestClient, BeholderRestClientContext } from '@services/BeholderRestClient';

import 'simple-keyboard/build/css/index.css';
import '../styles/simplekeyboard.css';

import '../styles/globals.css';
import '../styles/index.css';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
    },
  },
});

function BeholderApp({ Component, pageProps }) {
  const appStore = useAppStore(pageProps);
  const beholderClient = useRef(new BeholderClient(appStore.beholderStore));
  const beholderRestClient = useRef(new BeholderRestClient(appStore.beholderStore));

  useEffect(() => {
    if (!beholderClient.current.connected) {
      beholderClient.current.connect();
      setTimeout(beholderClient.current.pulse, 2500);
    }
  }, []);

  useEffect(() => {
    if (!beholderRestClient.current.baseUrl) {
      beholderRestClient.current.initialize();
    }
  }, []);

  return (
    <AppStoreContext.Provider value={appStore}>
      <QueryClientProvider client={queryClient}>
        <BeholderClientContext.Provider value={beholderClient.current}>
          <BeholderRestClientContext.Provider value={beholderRestClient.current}>
            <Component {...pageProps} />
          </BeholderRestClientContext.Provider>
        </BeholderClientContext.Provider>
      </QueryClientProvider>
    </AppStoreContext.Provider>
  );
}

export default observer(BeholderApp);
