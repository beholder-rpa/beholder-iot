import { BeholderEyeInfo } from './eye';
import { MatrixPixelLocation } from './eye/ObservationRequest';
import { ProcessInfo } from './psionix/ProcessInfo';

export interface EyeState {
  eyeAlignmentMap: MatrixPixelLocation[];
  processList: ProcessInfo[];
  eyeInfo: BeholderEyeInfo;
  foregroundProcess?: ProcessInfo;
  observedProcesses: string[];
  lastMatrixFrame?: any;
}

export const defaultEyeState: EyeState = {
  eyeAlignmentMap: [],
  processList: [],
  eyeInfo: {
    status: 'Unknown',
  },
  observedProcesses: [],
};
