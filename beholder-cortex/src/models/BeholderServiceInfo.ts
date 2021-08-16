import { Dayjs } from 'dayjs';

export interface BeholderServiceInfo {
  key?: string;
  hostName: string;
  ipAddresses: string;
  serviceName: string;
  version: string;
  lastSeen: Dayjs;
}
