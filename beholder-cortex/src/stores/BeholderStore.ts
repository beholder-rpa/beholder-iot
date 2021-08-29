import { observable, action, autorun, makeAutoObservable, runInAction } from 'mobx';
import dayjs from 'dayjs';
import { merge } from 'lodash';

import {
  BeholderDaemonServiceInfo,
  BeholderServiceInfo,
  PointerImage,
  PointerPosition,
  RegionCaptureInfo,
} from '@models/BeholderServiceInfo';

import { AppStore } from './AppStore';

class BeholderStore {
  appStore: AppStore;

  @observable isConnected = false;
  @observable lastMessageReceived: string = null;
  @observable secondsSinceLastMessageRecieved = null;
  @observable beholderServices: { [hostName: string]: BeholderServiceInfo } = {};

  constructor(appStore: AppStore) {
    makeAutoObservable(this);
    this.appStore = appStore;

    autorun(
      () => {
        runInAction(() => {
          this.updateSecondsSinceLastMessageRecieved();
        });
      },
      {
        scheduler: (run) => setInterval(run, 1000),
      },
    );
  }

  @action putBeholderService(beholderService: BeholderServiceInfo) {
    const key = `${beholderService.serviceName}-${beholderService.hostName}`;
    beholderService.key = key;
    beholderService.lastSeen = dayjs();
    const existingService = this.beholderServices[key];
    if (!existingService) {
      this.beholderServices[key] = observable(beholderService);
      return;
    }

    merge(existingService, beholderService);
  }

  @action addUpdateRegion(
    beholderService: BeholderServiceInfo,
    regionName: string,
    regionCaptureInfo: RegionCaptureInfo,
  ) {
    const existingDaemonService: BeholderDaemonServiceInfo =
      this.beholderServices[`${beholderService.serviceName}-${beholderService.hostName}`];

    if (!existingDaemonService || existingDaemonService.serviceName !== 'daemon') {
      return;
    }

    if (!existingDaemonService.regions) {
      existingDaemonService.regions = {};
    }

    const existingRegion = existingDaemonService.regions[regionName];
    if (!existingRegion) {
      existingDaemonService.regions[regionName] = observable(regionCaptureInfo);
      return;
    }

    merge(existingRegion, regionCaptureInfo);
  }

  @action updateLastPointerPosition(beholderService: BeholderServiceInfo, position: PointerPosition) {
    const existingDaemonService: BeholderDaemonServiceInfo =
      this.beholderServices[`${beholderService.serviceName}-${beholderService.hostName}`];

    if (!existingDaemonService || existingDaemonService.serviceName !== 'daemon') {
      return;
    }

    existingDaemonService.pointerPosition = observable(position);
  }

  @action updateLastPointerImage(beholderService: BeholderServiceInfo, img: PointerImage) {
    const existingDaemonService: BeholderDaemonServiceInfo =
      this.beholderServices[`${beholderService.serviceName}-${beholderService.hostName}`];

    if (!existingDaemonService || existingDaemonService.serviceName !== 'daemon') {
      return;
    }

    existingDaemonService.pointerImage = observable(img);
  }

  @action updateLastHotkeyPressed(beholderService: BeholderServiceInfo, hotKey: string) {
    const existingDaemonService: BeholderDaemonServiceInfo =
      this.beholderServices[`${beholderService.serviceName}-${beholderService.hostName}`];

    if (!existingDaemonService || existingDaemonService.serviceName !== 'daemon') {
      return;
    }

    existingDaemonService.lastHotKeyPressed = observable({
      hotKey: hotKey,
      time: dayjs(),
    });
  }

  @action updateConnected(isConnected: boolean) {
    this.isConnected = isConnected;
  }

  @action updateLastMessageRecieved() {
    this.lastMessageReceived = new Date().toISOString();
  }

  @action updateSecondsSinceLastMessageRecieved() {
    const lastMessageReceived = dayjs(this.lastMessageReceived);
    this.secondsSinceLastMessageRecieved = dayjs().diff(lastMessageReceived, 'seconds');
  }
}

export default BeholderStore;
