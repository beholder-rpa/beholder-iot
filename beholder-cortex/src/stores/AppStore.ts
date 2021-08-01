import React, { useMemo } from 'react';
import { action, makeAutoObservable, observable } from 'mobx';
import { enableStaticRendering } from 'mobx-react';

import UIStore from './UIStore';
import BeholderStore from './BeholderStore';

// eslint-disable-next-line react-hooks/rules-of-hooks
enableStaticRendering(typeof window === 'undefined');
let appStore: AppStore;

export class AppStore {
  @observable lastUpdate: Date;
  @observable beholderStore: BeholderStore;
  @observable uiStore: UIStore;

  constructor() {
    this.beholderStore = new BeholderStore(this);
    this.uiStore = new UIStore(this);
    makeAutoObservable(this);
  }

  @action hydrate = (initialProps: any) => {
    if (!initialProps) return;

    this.lastUpdate = initialProps.lastUpdate !== null ? initialProps.lastUpdate : Date.now();
  };
}

function initializeStore(initialProps = null) {
  const _appStore = appStore ?? new AppStore();

  if (initialProps) {
    _appStore.hydrate(initialProps);
  }

  // For SSG and SSR always create a new store
  if (typeof window === 'undefined') return _appStore;

  // Create the store once in the client
  if (!appStore) appStore = _appStore;

  return _appStore;
}

export function useAppStore(initialProps?: any) {
  const store = useMemo(() => initializeStore(initialProps), [initialProps]);
  return store;
}

export const AppStoreContext = React.createContext<AppStore>(appStore);
