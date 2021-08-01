export default interface Keypress {
  key: string;
  direction?: KeyDirection;
  modifiers?: string[];
  duration?: Duration;
}

export enum KeyDirection {
  PressAndRelease = 0,
  Press = 1,
  Release = 2,
}

export interface Duration {
  delay?: number;
  min?: number;
  max?: number;
}
