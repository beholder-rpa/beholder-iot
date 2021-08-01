import { BeholderClient } from '@services/BeholderClient';
import BeholderStore from '@stores/BeholderStore';

export interface BeholderEyeEventHandler {
  [eventName: string]: (
    event: MatrixEvent,
    host: string,
    store: BeholderStore,
    client: BeholderClient,
  ) => Promise<void>;
}

export interface MatrixFrame {
  /**
   * Id of the frame
   */
  id: string;
  /**
   * Frame metadata
   */
  metadata: number[];
  /**
   * Frame Time
   */
  ft: string;
  /**
   * Data
   */
  d: any;
}

export interface MatrixEvent {
  /**
   * Topic
   */
  t: string;
  /**
   * Subject
   */
  s: string;
  /**
   * Event Time
   */
  et: string;
  /**
   * Data
   */
  d: any;
  /**
   * Priority
   */
  p: string;
}

export interface BeholderEyeInfo {
  status: string;
}
