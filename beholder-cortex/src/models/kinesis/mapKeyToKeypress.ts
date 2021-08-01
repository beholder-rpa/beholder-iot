import Keypress from '@src/models/kinesis/Keypress';

const mapKeyToKeypress = (key: string): Keypress => {
  const result: Keypress = {
    key: '',
  };

  if (key.match(/[SCA]?[0-9-=]/)) {
    let modifier = null,
      toprow = key[0];

    if (key.length === 2) {
      modifier = key[0];
      toprow = key[1];
    }

    switch (modifier) {
      case 'S':
        result.modifiers = ['left-shift'];
        break;
      case 'C':
        result.modifiers = ['left-ctrl'];
        break;
      case 'A':
        result.modifiers = ['left-alt'];
        break;
    }

    switch (toprow) {
      case '-':
        result.key = 'dash';
        break;
      case '=':
        result.key = 'equals';
        break;
      default:
        result.key = toprow;
        break;
    }
  }

  return result;
};

export default mapKeyToKeypress;
