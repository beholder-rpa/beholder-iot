import { Dayjs as Moment } from 'dayjs';

export interface ProcessInfo {
  id: number;
  processName: string;
  mainWindowTitle: string;
  startTime: string;
  processStatus: string;
  windowPlacement: string;
  workingSet64: number;
  lastActive?: Moment;
}
