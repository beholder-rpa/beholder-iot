import React from 'react';

import BeholderStore from '@stores/BeholderStore';

export class BeholderRestClient {
  private baseUrlInternal: string;

  constructor(readonly beholderStore: BeholderStore) {}

  get baseUrl() {
    return this.baseUrlInternal;
  }

  initialize() {
    this.baseUrlInternal = `//${window?.location.host}`;

    if (process.env.NODE_ENV !== 'production') {
      this.baseUrlInternal = `//localhost:5271`;
    }
  }
}

export const BeholderRestClientContext = React.createContext<BeholderRestClient>(null);
