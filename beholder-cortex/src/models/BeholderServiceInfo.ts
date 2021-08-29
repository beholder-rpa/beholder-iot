import { Dayjs } from 'dayjs';

export interface BeholderServiceInfo {
  key: string;
  hostName: string;
  ipAddresses: string;
  serviceName: 'daemon' | 'occipital' | 'epidermis' | 'cortex' | 'stalk';
  version: string;
  lastSeen: Dayjs;
}

export interface BeholderDaemonServiceInfo extends BeholderServiceInfo {
  pointerPosition?: PointerPosition;
  pointerImage?: PointerImage;
  lastHotKeyPressed?: {
    hotKey: string;
    time: Dayjs;
  };
  regions?: { [name: string]: RegionCaptureInfo };
}

export interface PointerPosition {
  x: number;
  y: number;
  v: boolean;
}

export interface PointerImage {
  image: string;
  key: string;
}

export interface RegionCaptureInfo {
  prefrontalImageKey: string;
  x: number;
  y: number;
  width: number;
  height: number;
}
