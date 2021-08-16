import { observable, action, autorun, makeAutoObservable, runInAction } from 'mobx';
import dayjs from 'dayjs';
import { merge, find } from 'lodash';

import { defaultEyeState, EyeState } from '@src/models/EyeState';
import { BeholderEyeInfo, MatrixEvent } from '@src/models/eye';
import { defaultKinesisState, KinesisState } from '@src/models/KinesisState';
import { MatrixPixelLocation } from '@src/models/eye/ObservationRequest';
import { ProcessInfo } from '@src/models/psionix/ProcessInfo';
import { HostInfo } from '@src/models/cortex/HostInfo';
import { BeholderServiceInfo } from '@models/BeholderServiceInfo';

import { AppStore } from './AppStore';

class BeholderStore {
  appStore: AppStore;

  @observable isConnected = false;
  @observable lastMessageReceived: string = null;
  @observable secondsSinceLastMessageRecieved = null;
  @observable hosts: HostInfo[] = [];
  @observable serviceInfo: BeholderServiceInfo[] = [];
  @observable kinesisHosts: HostInfo[] = [];
  @observable eyeState: Record<string, EyeState> = {};
  @observable kinesisState: Record<string, KinesisState> = {};
  @observable currentRotation?: string;

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

  @action addHost(host: HostInfo) {
    const existingHost = find(this.hosts, { name: host.name });
    if (!existingHost) {
      this.hosts.push(observable(host));
      return;
    }

    let existingHostData = this.eyeState[host.name];
    if (!existingHostData) {
      this.eyeState[host.name] = existingHostData = observable(defaultEyeState);
    }

    merge(existingHost, host);
  }

  @action putServiceInfo(serviceInfo: BeholderServiceInfo) {
    serviceInfo.key = `${serviceInfo.hostName}-${serviceInfo.serviceName}`;
    serviceInfo.lastSeen = dayjs();
    const existingService = find(this.serviceInfo, { key: serviceInfo.key });
    if (!existingService) {
      this.serviceInfo.push(observable(serviceInfo));
      return;
    }

    merge(existingService, serviceInfo);
  }

  @action addKinesisHost(kinesisHost: HostInfo) {
    const existingHost = find(this.kinesisHosts, { name: kinesisHost.name });
    if (!existingHost) {
      this.kinesisHosts.push(observable(kinesisHost));
      return;
    }

    let existingKinesisState = this.kinesisState[kinesisHost.name];
    if (!existingKinesisState) {
      this.kinesisState[kinesisHost.name] = existingKinesisState = observable(defaultKinesisState);
    }

    merge(existingHost, kinesisHost);
  }

  @action updateHostProcessList(host: string, processList: ProcessInfo[]) {
    const hostData = this.getEyeState(host);

    hostData.processList = observable(processList);
  }

  @action updateHostProcess(host: string, process: ProcessInfo) {
    const hostData = this.getEyeState(host);

    if (!hostData.processList) {
      hostData.processList = observable([]);
    }

    const existingProcess = find(hostData.processList, { processName: process.processName });
    if (existingProcess) {
      if (existingProcess.processStatus === 'Active' && process.processStatus !== 'Active') {
        process.lastActive = dayjs();
      } else {
        process.lastActive = undefined;
      }
      merge(existingProcess, process);
    } else {
      hostData.processList.push(observable(process));
    }
  }

  @action updateHostForegroundProcess(host: string, foregroundProcess: ProcessInfo) {
    const hostData = this.getEyeState(host);

    hostData.foregroundProcess = observable(foregroundProcess);
  }

  @action updateObservedProcesses(host: string, observedProcesses: string[]) {
    const hostData = this.getEyeState(host);

    hostData.observedProcesses = observable(observedProcesses);
  }

  @action updateEyeStatus(host: string, info: BeholderEyeInfo) {
    const hostData = this.getEyeState(host);

    hostData.eyeInfo = observable(info);
  }

  @action updateEyeAlignmentMap(host: string, alignmentMap: MatrixPixelLocation[]) {
    const hostData = this.getEyeState(host);

    hostData.eyeAlignmentMap = observable(alignmentMap);
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

  @action getFirstHost() {
    if (this.hosts.length < 1) {
      return undefined;
    }

    return this.hosts[0];
  }

  @action getFirstKinesisHost() {
    if (this.kinesisHosts.length < 1) {
      return undefined;
    }

    return this.kinesisHosts[0];
  }

  @action getEyeState(hostName: string) {
    let existingEyeState = this.eyeState[hostName];
    if (!existingEyeState) {
      this.eyeState[hostName] = existingEyeState = observable(defaultEyeState);
    }
    return existingEyeState;
  }

  @action getKinesisState(hostName: string) {
    let existingKinesisState = this.kinesisState[hostName];
    if (!existingKinesisState) {
      this.kinesisState[hostName] = existingKinesisState = observable(defaultKinesisState);
    }
    return existingKinesisState;
  }

  @action async updateLastMatrixFrame(host: string, matrixFrame: any) {
    const hostData = this.getEyeState(host);

    hostData.lastMatrixFrame = observable(matrixFrame);

    // Special Stuffs
    for (const event of matrixFrame.d as MatrixEvent[]) {
      console.log(`Received topic '${event.t}' but a associated handler function could not be found`);
    }
  }

  @action async toggleRotation(rotationName: string) {
    if (this.currentRotation === rotationName) {
      this.currentRotation = '';
    } else {
      this.currentRotation = rotationName;
    }
  }
}

export default BeholderStore;
