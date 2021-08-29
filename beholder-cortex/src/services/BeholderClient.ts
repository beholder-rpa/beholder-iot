import React from 'react';
import mqtt, { IMqttClient } from 'async-mqtt';

import BeholderStore from '@stores/BeholderStore';

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

    this.client = mqtt.connect(`wss://${host}/nexus/mqtt`, {
      username: this.username,
      password: this.authToken,
      reconnectPeriod: 5000,
      keepalive: 15,
    });

    this.client.on('connect', () => {
      console.log('%c connected!!', 'background: green; color: white; display: block;');
      // Subscribe to all interesting topics
      this.client.subscribe([
        'beholder/ctaf',
        'beholder/eye/+/region/+',
        'beholder/eye/+/pointer_position',
        'beholder/eye/+/pointer_image',
        'beholder/psionix/+/hotkeys/pressed/+',
      ]);
      this.pulse();

      this.beholderStore.updateConnected(true);
      this.beholderStore.updateLastMessageRecieved();
    });

    this.client.on('message', (topic, message) => {
      const strMessage = message.toString('utf8');
      const cloudEvent = JSON.parse(strMessage);
      switch (topic) {
        case `beholder/ctaf`:
          this.beholderStore.putBeholderService(cloudEvent.data);
          break;
      }

      // Process host-service specific messages
      for (const knownService of Object.values(this.beholderStore.beholderServices)) {
        switch (topic) {
          case `beholder/eye/${knownService.hostName}/pointer_position`:
            this.beholderStore.updateLastPointerPosition(knownService, cloudEvent.data);
            break;
          case `beholder/eye/${knownService.hostName}/pointer_image`:
            this.beholderStore.updateLastPointerImage(knownService, cloudEvent.data);
            break;
        }

        if (topic.startsWith(`beholder/eye/${knownService.hostName}/region/`)) {
          this.beholderStore.addUpdateRegion(
            knownService,
            topic.replace(`beholder/eye/${knownService.hostName}/region/`, ''),
            cloudEvent.data,
          );
        }

        if (topic.startsWith(`beholder/psionix/${knownService.hostName}/hotkeys/pressed/`)) {
          this.beholderStore.updateLastHotkeyPressed(knownService, cloudEvent.data);
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
}

export const BeholderClientContext = React.createContext<BeholderClient>(null);
