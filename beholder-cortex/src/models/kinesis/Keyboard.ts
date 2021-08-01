import { hesitate, getDelayMs, rand } from '@src/utils';
import { BeholderClient } from '@services/BeholderClient';

const defaultKeyPressThinkOptions = {
  min: 57,
  max: 210,
};

const clearReport = Buffer.alloc(8);

// See https://github.com/Oceanswave/pi-as-keyboard/blob/master/hid-gadget-test.c
export class Keyboard {
  private client: BeholderClient;
  private hostName: string;
  private sendKeysRawTopic = 'k/{0}/keyboard/raw';
  constructor(client: BeholderClient, hostName: string) {
    this.client = client;
    this.hostName = hostName;
    this.sendKeysRawTopic = this.sendKeysRawTopic.replace('{0}', hostName);
  }

  private getModifiers(mods?: string[]): number {
    let modifierBytes = 0x00;
    if (mods) {
      for (const key of mods) {
        if (modifiers[key]) {
          modifierBytes = modifierBytes | modifiers[key];
        }
      }
    }

    return modifierBytes;
  }

  /**
   * Sends a single keypress (down-hesitate-up)
   * @param keypress
   */
  public async keypress(keypress: Keypress) {
    const keyValue = keys[keypress.key];
    if (!keyValue) {
      return;
    }

    const modifierBytes = this.getModifiers(keypress.modifiers);

    //TODO need to refactor send report on the nexus to be able to retrieve hand connection ids for sendreport.
    this.client.mqtt.publish(
      this.sendKeysRawTopic,
      Buffer.from([modifierBytes, 0x00, keyValue, 0x00, 0x00, 0x00, 0x00, 0x00]),
    );
    await hesitate(keypress.duration || defaultKeyPressThinkOptions);
    this.client.mqtt.publish(this.sendKeysRawTopic, clearReport);
  }

  public async sendKeys(keypresses: Keypress[]) {
    if (!keypresses || keypresses.length <= 0) {
      throw Error('At least one keypress must be provided.');
    }

    if (keypresses.length > 5) {
      throw Error('Maximum of 5 simultaneous keydowns');
    }

    // Use the modifiers from the first key in the sequence.
    const modifierBytes = this.getModifiers(keypresses[0].modifiers);

    let ix = 2;
    const promises = [];
    const report = [modifierBytes, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
    const sendKeyPromise = (keypress: Keypress, ix: number) => {
      return new Promise(async (resolve, _) => {
        if (!keys[keypress.key]) {
          return;
        }

        const keycode = keys[keypress.key];
        const startMs = getDelayMs(keypress.start);
        const durationMs = getDelayMs(keypress.duration);
        let repeats = 0;
        if (keypress.repeats) {
          repeats = rand(keypress.repeats.min, keypress.repeats.max);
        }

        setTimeout(
          async (report, ix) => {
            report[ix] = keycode;
            this.client.mqtt.publish(this.sendKeysRawTopic, Buffer.from(report));

            setTimeout(
              async (report, ix) => {
                report[ix] = 0x00;
                this.client.mqtt.publish(this.sendKeysRawTopic, Buffer.from(report));
                for (let r = 0; r < repeats; r++) {
                  await sendKeyPromise({ ...keypress, repeats: undefined }, ix);
                }
                resolve(null);
              },
              durationMs,
              report,
              ix,
            );
          },
          startMs,
          report,
          ix,
        );
      });
    };

    // Get the absolute durations for each keypress
    for (const keypress of keypresses) {
      const promise = sendKeyPromise(keypress, ix);
      promises.push(promise);
      for (let r = 0; r < keypress.repeats; r++) {
        const promise = sendKeyPromise(keypress, ix);
        promises.push(promise);
      }
      ix++;
    }

    try {
      await Promise.all(promises);
    } catch (err) {
      this.client.mqtt.publish(this.sendKeysRawTopic, JSON.stringify([clearReport]));
      throw err;
    }
  }
}

export const modifiers = {
  'left-ctrl': 0x01,
  'right-ctrl': 0x10,
  'left-shift': 0x02,
  'right-shift': 0x20,
  'left-alt': 0x04,
  'right-alt': 0x40,
  'left-meta': 0x08,
  'right-meta': 0x80,
};

export const keys = {
  a: 0x04,
  b: 0x05,
  c: 0x06,
  d: 0x07,
  e: 0x08,
  f: 0x09,
  g: 0x0a,
  h: 0x0b,
  i: 0x0c,
  j: 0x0d,
  k: 0x0e,
  l: 0x0f,
  m: 0x10,
  n: 0x11,
  o: 0x12,
  p: 0x13,
  q: 0x14,
  r: 0x15,
  s: 0x16,
  t: 0x17,
  u: 0x18,
  v: 0x19,
  w: 0x1a,
  x: 0x1b,
  y: 0x1c,
  z: 0x1d,

  '1': 0x1e,
  '2': 0x1f,
  '3': 0x20,
  '4': 0x21,
  '5': 0x22,
  '6': 0x23,
  '7': 0x24,
  '8': 0x25,
  '9': 0x26,
  '0': 0x27,

  return: 0x28,
  enter: 0x28,
  esc: 0x29,
  escape: 0x29,
  bckspc: 0x2a,
  backspace: 0x2a,
  tab: 0x2b,
  space: 0x2c,
  minus: 0x2d,
  dash: 0x2d,
  equals: 0x2e,
  equal: 0x2e,
  lbracket: 0x2f,
  rbracket: 0x30,
  backslash: 0x31,
  hash: 0x32,
  number: 0x32,
  semicolon: 0x33,
  quote: 0x34,
  backquote: 0x35,
  tilde: 0x35,
  comma: 0x36,
  period: 0x37,
  stop: 0x37,
  slash: 0x38,
};

export interface Keypress {
  key: string;
  modifiers?: string[];
  start?: { delay?: number; min?: number; max?: number };
  duration?: { delay?: number; min?: number; max?: number };
  repeats?: { min?: number; max?: number };
}
