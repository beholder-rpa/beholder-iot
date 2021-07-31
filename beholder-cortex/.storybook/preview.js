import { addDecorator } from '@storybook/react';
import { RouterContext } from 'next/dist/next-server/lib/router-context';
import { initialize, mswDecorator } from 'msw-storybook-addon';

import '../src/styles/globals.css';
import '../src/styles/index.css';

initialize();
export const parameters = {
  actions: { argTypesRegex: "^on[A-Z].*" },
  controls: {
    matchers: {
      color: /(background|color)$/i,
      date: /Date$/,
    },
  },
  nextRouter: {
    Provider: RouterContext.Provider,
  },
}

addDecorator(mswDecorator);
