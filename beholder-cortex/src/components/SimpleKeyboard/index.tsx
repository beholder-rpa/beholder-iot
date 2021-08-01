import React, { useEffect, useRef } from 'react';
import Keyboard from 'simple-keyboard';

import { KeyboardOptions } from './SimpleKeyboardOptions';

const SimpleKeyboard = (props: KeyboardOptions) => {
  const keyboard = useRef<Keyboard>();
  const keyboardControlPad = useRef<Keyboard>();
  const keyboardArrows = useRef<Keyboard>();
  const keyboardNumPad = useRef<Keyboard>();
  const keyboardNumPadEnd = useRef<Keyboard>();

  const commonKeyboardOptions: KeyboardOptions = {
    onKeyPress: (button) => onKeyPress(button),
    theme: 'simple-keyboard hg-theme-default hg-layout-default myTheme1',
    physicalKeyboardHighlight: true,
    syncInstanceInputs: true,
    mergeDisplay: true,
  };

  const handleShift = () => {
    const currentLayout = keyboard.current.options.layoutName;
    const shiftToggle = currentLayout === 'default' ? 'shift' : 'default';

    keyboard.current.setOptions({
      layoutName: shiftToggle,
    });
  };

  const onKeyPress = (button: string) => {
    if (button === '{shift}' || button === '{shiftleft}' || button === '{shiftright}' || button === '{capslock}')
      handleShift();
  };

  useEffect(() => {
    import('simple-keyboard').then((KeyboardClass) => {
      const Keyboard = KeyboardClass.default;
      keyboard.current = new Keyboard('.simple-keyboard-main', {
        ...commonKeyboardOptions,
        layout: {
          default: [
            '{escape} {f1} {f2} {f3} {f4} {f5} {f6} {f7} {f8} {f9} {f10} {f11} {f12}',
            '` 1 2 3 4 5 6 7 8 9 0 - = {backspace}',
            '{tab} q w e r t y u i o p [ ] \\',
            "{capslock} a s d f g h j k l ; ' {enter}",
            '{shiftleft} z x c v b n m , . / {shiftright}',
            '{controlleft} {altleft} {metaleft} {space} {metaright} {altright}',
          ],
          shift: [
            '{escape} {f1} {f2} {f3} {f4} {f5} {f6} {f7} {f8} {f9} {f10} {f11} {f12}',
            '~ ! @ # $ % ^ & * ( ) _ + {backspace}',
            '{tab} Q W E R T Y U I O P { } |',
            '{capslock} A S D F G H J K L : " {enter}',
            '{shiftleft} Z X C V B N M < > ? {shiftright}',
            '{controlleft} {altleft} {metaleft} {space} {metaright} {altright}',
          ],
        },
        display: {
          '{escape}': 'esc ⎋',
          '{tab}': 'tab ⇥',
          '{backspace}': 'backspace ⌫',
          '{enter}': 'enter ↵',
          '{capslock}': 'caps lock ⇪',
          '{shiftleft}': 'shift ⇧',
          '{shiftright}': 'shift ⇧',
          '{controlleft}': 'ctrl ⌃',
          '{controlright}': 'ctrl ⌃',
          '{altleft}': 'alt ⌥',
          '{altright}': 'alt ⌥',
          '{metaleft}': 'cmd ⌘',
          '{metaright}': 'cmd ⌘',
        },
        ...props,
      });
      keyboardControlPad.current = new Keyboard('.simple-keyboard-control', {
        ...commonKeyboardOptions,
        layout: {
          default: ['{prtscr} {scrolllock} {pause}', '{insert} {home} {pageup}', '{delete} {end} {pagedown}'],
        },
        ...props,
      });
      keyboardArrows.current = new Keyboard('.simple-keyboard-arrows', {
        ...commonKeyboardOptions,
        layout: {
          default: ['{arrowup}', '{arrowleft} {arrowdown} {arrowright}'],
        },
        ...props,
      });

      keyboardNumPad.current = new Keyboard('.simple-keyboard-numpad', {
        ...commonKeyboardOptions,
        layout: {
          default: [
            '{numlock} {numpaddivide} {numpadmultiply}',
            '{numpad7} {numpad8} {numpad9}',
            '{numpad4} {numpad5} {numpad6}',
            '{numpad1} {numpad2} {numpad3}',
            '{numpad0} {numpaddecimal}',
          ],
        },
        ...props,
      });

      keyboardNumPadEnd.current = new Keyboard('.simple-keyboard-numpadEnd', {
        ...commonKeyboardOptions,
        layout: {
          default: ['{numpadsubtract}', '{numpadadd}', '{numpadenter}'],
        },
        ...props,
      });
    });
  });

  return (
    <div className="keyboardContainer">
      <div className="simple-keyboard-main"></div>

      <div className="controlArrows">
        <div className="simple-keyboard-control"></div>
        <div className="simple-keyboard-arrows"></div>
      </div>

      <div className="numPad">
        <div className="simple-keyboard-numpad"></div>
        <div className="simple-keyboard-numpadEnd"></div>
      </div>
    </div>
  );
};

export default SimpleKeyboard;
