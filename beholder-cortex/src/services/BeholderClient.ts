import React from 'react';
import mqtt, { IMqttClient } from 'async-mqtt';

import BeholderStore from '@stores/BeholderStore';
import { ObservationRequest } from '@src/models/eye/ObservationRequest';

export class BeholderClient {
  private client: IMqttClient;
  private username: string;
  private authToken: string;

  constructor(readonly beholderStore: BeholderStore) {}

  get mqtt(): IMqttClient {
    return this.client;
  }

  get connected(): boolean {
    if (!this.client) {
      return false;
    }

    return this.client.connected;
  }

  connect() {
    const host = window.location.host;
    this.username = 'guest';
    this.authToken = 'guest';

    this.client = mqtt.connect(`wss://${host}/nexus/ws`, {
      username: this.username,
      password: this.authToken,
      reconnectPeriod: 5000,
      keepalive: 15,
    });

    this.client.on('connect', () => {
      console.log('%c connected!!', 'background: green; color: white; display: block;');
      this.client.subscribe('beholder/+');
      this.pulse();

      this.client.publish('beholder/ping', null);
      this.beholderStore.updateConnected(true);
      this.beholderStore.updateLastMessageRecieved();
    });

    this.client.on('message', (topic, message) => {
      const strMessage = message.toString('utf8');
      switch (topic) {
        case `beholder/host`:
          const host = JSON.parse(strMessage);
          this.beholderStore.addHost(host);

          // Now that we have a host name, Subscribe to host-specific messages
          this.client.subscribe(`e/${host.name}/status`); // Subscribe to status updates.
          this.client.subscribe(`e/${host.name}/alignment_map`); // Subscribe to alignment map updates.
          this.client.subscribe(`e/${host.name}/matrix_frame`); // Subscribe to matrix frame updates

          this.client.subscribe(`p/${host.name}/+`); // Subscribe to all psionix updates

          // Also request some info
          this.client.publish(`e/${host.name}/report_status`, null);
          this.client.publish(`p/${host.name}/request_foreground_process`, null);
          this.client.publish(`p/${host.name}/request_observed_process`, null);
          break;
        case `beholder/kinesis`:
          const kinesisHost = JSON.parse(strMessage);
          this.beholderStore.addKinesisHost(kinesisHost);

          // Now that we have a kinesis host, subscribe to kinesis-specific messages
          this.client.subscribe(`k/${kinesisHost.name}/status/keyboard/leds`); // Subscribe to keyboard led changes
          break;
      }

      // Process Host-Specific Messages (responses usually)
      for (const host of this.beholderStore.hosts) {
        switch (topic) {
          // Psyonix
          case `p/${host.name}/process_list`:
            const processes = JSON.parse(strMessage);
            this.beholderStore.updateHostProcessList(host.name, processes);
            break;
          case `p/${host.name}/process_changed`:
            const processInfo = JSON.parse(strMessage);
            this.beholderStore.updateHostProcess(host.name, processInfo);
            break;
          case `p/${host.name}/foreground_process`:
            const foregroundProcessInfo = JSON.parse(strMessage);
            this.beholderStore.updateHostForegroundProcess(host.name, foregroundProcessInfo);
            break;
          case `p/${host.name}/observed_processes`:
            const observedProcesses = JSON.parse(strMessage);
            this.beholderStore.updateObservedProcesses(host.name, observedProcesses);
            break;
          // Eye
          case `e/${host.name}/status`:
            const beholderEyeInfo = JSON.parse(strMessage);
            this.beholderStore.updateEyeStatus(host.name, beholderEyeInfo);
            break;
          case `e/${host.name}/alignment_map`:
            const alignmentMap = JSON.parse(strMessage);
            this.beholderStore.updateEyeAlignmentMap(host.name, alignmentMap);
            break;
          case `e/${host.name}/matrix_frame`:
            const matrixFrame = JSON.parse(strMessage);
            this.beholderStore.updateLastMatrixFrame(host.name, matrixFrame);
            break;
        }
      }

      // Process Kinesis Host-Specific Messages (responses usually)
      for (const host of this.beholderStore.kinesisHosts) {
        switch (topic) {
          // Kinesis
          case `k/${host.name}/status/keyboard/leds`:
            console.dir(strMessage);
            break;
        }
      }

      this.beholderStore.updateLastMessageRecieved();
    });

    this.client.on('error', (err) => {
      this.beholderStore.updateConnected(false);
      console.log('Broker reported error: ' + err.message);
      console.log('Additional details: ' + err.stack);
    });
  }

  disconnect() {
    this.client?.end();
  }

  requestClientStatusUpdate() {
    if (!this.client) {
      return;
    }

    this.client.publish(`u/${this.username}/c/notifications`, JSON.stringify({ message: 'report_status' }));
  }

  pulse() {
    if (!this.client) {
      return;
    }

    this.client.publish(`u/${this.username}/w/ident`, null);
  }

  /**
   * Sends a RPC-Style message that requests that the indicated host commuicates the list of current processes.
   */
  requestProcessList() {
    if (!this.client) {
      return;
    }
    const firstHost = this.beholderStore.getFirstHost();
    if (!firstHost) {
      return;
    }
    this.client.publish(`p/${firstHost.name}/get_processes`, null);
  }

  /**
   * Sends a RPC-Style message that requests to start observing the screen
   */
  startObserving(hostName?: string, observationRequest: ObservationRequest = {}) {
    if (!this.client) {
      return;
    }

    if (!hostName) {
      const host = this.beholderStore.getFirstHost();
      if (host == null) {
        return;
      }
      hostName = host.name;
    }

    this.client.publish(`e/${hostName}/start_observing`, JSON.stringify(observationRequest));
  }

  /**
   * Sends a RPC-Style message that requests to stop observing the screen
   */
  stopObserving(hostName?: string) {
    if (!this.client) {
      return;
    }

    if (!hostName) {
      const host = this.beholderStore.getFirstHost();
      if (host == null) {
        return;
      }
      hostName = host.name;
    }

    this.client.publish(`e/${hostName}/stop_observing`, JSON.stringify({}));
  }

  requestAlignment(hostName?: string, pixelSize = 2) {
    if (!this.client) {
      return;
    }

    if (!hostName) {
      const host = this.beholderStore.getFirstHost();
      if (host == null) {
        return;
      }
      hostName = host.name;
    }

    this.client.publish(`e/${hostName}/request_align`, JSON.stringify({ pixelSize }));
  }

  startObservingProcess(hostName = '', processName: string) {
    if (!this.client) {
      return;
    }

    if (!hostName) {
      const host = this.beholderStore.getFirstHost();
      if (host == null) {
        return;
      }
      hostName = host.name;
    }

    this.client.publish(`p/${hostName}/observe_process`, processName);
  }

  stopObservingProcess(hostName = '', processName: string) {
    if (!this.client) {
      return;
    }

    if (!hostName) {
      const host = this.beholderStore.getFirstHost();
      if (host == null) {
        return;
      }
      hostName = host.name;
    }

    this.client.publish(`p/${hostName}/ignore_process`, processName);
  }

  ensureForegroundWindow(hostName = '', processName: string) {
    if (!this.client) {
      return;
    }

    if (!hostName) {
      const host = this.beholderStore.getFirstHost();
      if (host == null) {
        return;
      }
      hostName = host.name;
    }

    this.client.publish(`p/${hostName}/ensure_foreground_window`, processName);
  }
}

export const BeholderClientContext = React.createContext<BeholderClient>(null);
