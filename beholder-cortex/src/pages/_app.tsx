import { observer } from 'mobx-react';
import { QueryClient, QueryClientProvider } from 'react-query';

import { useAppStore, AppStoreContext } from '@stores/AppStore';

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

  return (
    <AppStoreContext.Provider value={appStore}>
      <QueryClientProvider client={queryClient}>
        <Component {...pageProps} />
      </QueryClientProvider>
    </AppStoreContext.Provider>
  );
}

export default observer(BeholderApp);
