namespace beholder_stalk
{
  using System;
  using System.Collections.Generic;

  public partial class Keyboard
  {
    public static byte[] ClearReport = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    // See:
    // https://www.usb.org/sites/default/files/documents/hut1_12v2.pdf
    // https://gist.github.com/MightyPork/6da26e382a7ad91b5496ee55fdc73db2
    // https://github.com/Oceanswave/pi-as-keyboard/blob/master/hid-gadget-test.c
    // Replace: \{\.opt\s*=\s*"([^"]+)",\s+\.val\s+=\s+([^}]+)\}, With: { "$1", $2 },
    public static IDictionary<string, (byte, KeyModifier?)> Keys = new Dictionary<string, (byte, KeyModifier?)>() {
            { "a", (0x04, null) }, // Keyboard a and A
            { "A", (0x04, KeyModifier.LeftShift) },
            { "b", (0x05, null) }, // Keyboard b and B
            { "B", (0x05, KeyModifier.LeftShift) },
            { "c", (0x06, null) }, // Keyboard c and C
            { "C", (0x06, KeyModifier.LeftShift) },
            { "d", (0x07, null) }, // Keyboard d and D
            { "D", (0x07, KeyModifier.LeftShift) },
            { "e", (0x08, null) }, // Keyboard e and E
            { "E", (0x08, KeyModifier.LeftShift) },
            { "f", (0x09, null) }, // Keyboard f and F
            { "F", (0x09, KeyModifier.LeftShift) },
            { "g", (0x0a, null) }, // Keyboard g and G
            { "G", (0x0a, KeyModifier.LeftShift) },
            { "h", (0x0b, null) }, // Keyboard h and H
            { "H", (0x0b, KeyModifier.LeftShift) },
            { "i", (0x0c, null) }, // Keyboard i and I
            { "I", (0x0c, KeyModifier.LeftShift) },
            { "j", (0x0d, null) }, // Keyboard j and J
            { "J", (0x0d, KeyModifier.LeftShift) },
            { "k", (0x0e, null) }, // Keyboard k and K
            { "K", (0x0e, KeyModifier.LeftShift) },
            { "l", (0x0f, null) }, // Keyboard l and L
            { "L", (0x0f, KeyModifier.LeftShift) },
            { "m", (0x10, null) }, // Keyboard m and M
            { "M", (0x10, KeyModifier.LeftShift) },
            { "n", (0x11, null) }, // Keyboard n and N
            { "N", (0x11, KeyModifier.LeftShift) },
            { "o", (0x12, null) }, // Keyboard o and O
            { "O", (0x12, KeyModifier.LeftShift) },
            { "p", (0x13, null) }, // Keyboard p and P
            { "P", (0x13, KeyModifier.LeftShift) },
            { "q", (0x14, null) }, // Keyboard q and Q
            { "Q", (0x14, KeyModifier.LeftShift) },
            { "r", (0x15, null) }, // Keyboard r and R
            { "R", (0x15, KeyModifier.LeftShift) },
            { "s", (0x16, null) }, // Keyboard s and S
            { "S", (0x16, KeyModifier.LeftShift) },
            { "t", (0x17, null) }, // Keyboard t and T
            { "T", (0x17, KeyModifier.LeftShift) },
            { "u", (0x18, null) }, // Keyboard u and U
            { "U", (0x18, KeyModifier.LeftShift) },
            { "v", (0x19, null) }, // Keyboard v and V
            { "V", (0x19, KeyModifier.LeftShift) },
            { "w", (0x1a, null) }, // Keyboard w and W
            { "W", (0x1a, KeyModifier.LeftShift) },
            { "x", (0x1b, null) }, // Keyboard x and X
            { "X", (0x1b, KeyModifier.LeftShift) },
            { "y", (0x1c, null) }, // Keyboard y and Y
            { "Y", (0x1c, KeyModifier.LeftShift) },
            { "z", (0x1d, null) }, // Keyboard z and Z
            { "Z", (0x1d, KeyModifier.LeftShift) },

            { "1", (0x1e, null) }, // Keyboard 1 and !
            { "!", (0x1e, KeyModifier.LeftShift) },
            { "2", (0x1f, null) }, // Keyboard 2 and @
            { "@", (0x1f, KeyModifier.LeftShift) },
            { "3", (0x20, null) }, // Keyboard 3 and #
            { "#", (0x20, KeyModifier.LeftShift) },
            { "4", (0x21, null) }, // Keyboard 4 and $
            { "$", (0x21, KeyModifier.LeftShift) },
            { "5", (0x22, null) }, // Keyboard 5 and %
            { "%", (0x22, KeyModifier.LeftShift) },
            { "6", (0x23, null) }, // Keyboard 6 and ^
            { "^", (0x23, KeyModifier.LeftShift) },
            { "7", (0x24, null) }, // Keyboard 7 and &
            { "&", (0x24, KeyModifier.LeftShift) },
            { "8", (0x25, null) }, // Keyboard 8 and *
            { "*", (0x25, KeyModifier.LeftShift) },
            { "9", (0x26, null) }, // Keyboard 9 and (
            { "(", (0x26, KeyModifier.LeftShift) },
            { "0", (0x27, null) }, // Keyboard 0 and )
            { ")", (0x27, KeyModifier.LeftShift) },

            { "return", (0x28, null) }, // Keyboard Return (ENTER)
            { "enter", (0x28, null) }, // Keyboard ESCAPE
            { "esc", (0x29, null) },
            { "escape", (0x29, null) },
            { "bckspc", (0x2a, null) }, // Keyboard DELETE (Backspace)
            { "backspace", (0x2a, null) },
            { "bs", (0x2a, null) },
            { "tab", (0x2b, null) }, // Keyboard Tab

            { " ", (0x2c, null) }, // Keyboard Spacebar
            { "space", (0x2c, null) },
            { "-", (0x2d, null) }, // Keyboard - and _
            { "minus", (0x2d, null) },
            { "dash", (0x2d, null) },
            { "_", (0x2d, KeyModifier.LeftShift) },
            { "underscore", (0x2d, KeyModifier.LeftShift) },
            { "=", (0x2e, null) }, // Keyboard = and +
            { "equals", (0x2e, null) },
            { "equal", (0x2e, null) },
            { "+", (0x2e, KeyModifier.LeftShift) },
            { "plus", (0x2e, KeyModifier.LeftShift) },
            { "[", (0x2f, null) }, // Keyboard [ and {
            { "lbracket", (0x2f, null) },
            { "lbrace", (0x2f, null) },
            { "{", (0x2f, KeyModifier.LeftShift) },
            { "lcurly", (0x2f, KeyModifier.LeftShift) },
            { "]", (0x30, null) }, // Keyboard ] and }
            { "rbracket", (0x30, null) },
            { "rbrace", (0x2f, null) },
            { "}", (0x30, KeyModifier.LeftShift) },
            { "rcurly", (0x30, KeyModifier.LeftShift) },
            { "\\", (0x31, null) }, // Keyboard \ and |
            { "backslash", (0x31, null) },
            { "|", (0x31, KeyModifier.LeftShift) },
            { "verticalbar", (0x31, KeyModifier.LeftShift) },
            { "vertical-bar", (0x31, KeyModifier.LeftShift) },
            { "pipe", (0x31, KeyModifier.LeftShift) },
            // { "#", (0x32, null) }, // Non-US # and ~
            // { "hash", (0x32, null) }, 
            // { "number", (0x32, null) },
            // { "~", (0x32, KeyModifier.LeftShift) }, 
            // { "tilde", (0x32, KeyModifier.LeftShift) }, 
            { ";", (0x33, null) }, // Keyboard ; and :
            { "semicolon", (0x33, null) },
            { ":", (0x33, KeyModifier.LeftShift) },
            { "colon", (0x33, KeyModifier.LeftShift) },
            { "'", (0x34, null) }, // Keyboard ' and "
            { "quote", (0x34, null) },
            { "\"", (0x34, KeyModifier.LeftShift) },
            { "doublequote", (0x34, KeyModifier.LeftShift) },
             { "double-quote", (0x34, KeyModifier.LeftShift) },
            { "`", (0x35, null) }, // Keyboard ` and ~
            { "backquote", (0x35, null) },
            { "grave", (0x35, null) },
            { "~", (0x35, KeyModifier.LeftShift) },
            { "tilde", (0x35, KeyModifier.LeftShift) },
            { ",", (0x36, null) }, // Keyboard , and <
            { "comma", (0x36, null) },
            { "<", (0x36, KeyModifier.LeftShift) },
            { "lt", (0x36, KeyModifier.LeftShift) },
            { "less-than", (0x36, KeyModifier.LeftShift) },
            { ".", (0x37, null) }, // Keyboard . and >
            { "period", (0x37, null) },
            { "dot", (0x37, null) },
            // { "stop", (0x37, null) },
            { ">", (0x37, KeyModifier.LeftShift) },
            { "gt", (0x37, KeyModifier.LeftShift) },
            { "greater-than", (0x37, KeyModifier.LeftShift) },
            { "/", (0x38, null) }, // Keyboard / and ?
            { "slash", (0x38, null) },
            { "?", (0x38, KeyModifier.LeftShift) },
            { "question", (0x38, KeyModifier.LeftShift) },
            { "questionmark", (0x38, KeyModifier.LeftShift) },

            { "capslock", (0x39, null) }, // Keyboard Caps Lock
	        { "caps-lock", (0x39, null) },

            { "f1", (0x3a, null) }, // Keyboard F1
            { "f2", (0x3b, null) }, // Keyboard F2
            { "f3", (0x3c, null) }, // Keyboard F3
            { "f4", (0x3d, null) }, // Keyboard F4
            { "f5", (0x3e, null) }, // Keyboard F5
            { "f6", (0x3f, null) }, // Keyboard F6
            { "f7", (0x40, null) }, // Keyboard F7
            { "f8", (0x41, null) }, // Keyboard F8
            { "f9", (0x42, null) }, // Keyboard F9
            { "f10", (0x43, null) }, // Keyboard F10
            { "f11", (0x44, null) }, // Keyboard F11
            { "f12", (0x45, null) }, // Keyboard F12

            { "sysrq", (0x46, null) }, // Keyboard Print Screen
            { "print", (0x46, null) },
            { "printscreen", (0x46, null) },
            { "print-screen", (0x46, null) },

            { "scrolllock", (0x47, null) }, // Keyboard Scroll Lock
            { "scroll-lock", (0x47, null) },

            { "pause", (0x48, null) }, // Keyboard Pause
            { "insert", (0x49, null) }, // Keyboard Insert
            { "home", (0x4a, null) }, // Keyboard Home
            { "pageup", (0x4b, null) }, // Keyboard Page Up
            { "pgup", (0x4b, null) },
            { "del", (0x4c, null) }, // Keyboard Delete Forward
            { "delete", (0x4c, null) },
            { "end", (0x4d, null) }, // Keyboard End
            { "pagedown", (0x4e, null) }, // Keyboard Page Down
            { "pgdown", (0x4e, null) },
            { "right", (0x4f, null) }, // Keyboard Right Arrow
            { "left", (0x50, null) }, // Keyboard Left Arrow
            { "down", (0x51, null) }, // Keyboard Down Arrow
            { "up", (0x52, null) }, // Keyboard Up Arrow

            { "num-lock", (0x53, null) }, // Keyboard Num Lock and Clear
            { "numlock", (0x53, null) },

            { "kp-slash", (0x54, null) }, // Keypad /
            { "kp-divide", (0x54, null) },
            { "numpaddiv", (0x54, null) },
            { "kp-multiply", (0x55, null) }, // Keypad *
            { "kp-asterisk", (0x55, null) },
            { "numpadmult", (0x55, null) },
            { "kp-minus", (0x56, null) }, // Keypad -
            { "numpadsub", (0x56, null) },
            { "kp-plus", (0x57, null) }, // Keypad +
            { "numpadadd", (0x57, null) },
            { "kp-enter", (0x58, null) }, // Keypad ENTER
            { "kp-return", (0x58, null) },
            { "numpadenter", (0x58, null) },
            { "kp-1", (0x59, null) }, // Keypad 1 and End
            { "kp-2", (0x5a, null) }, // Keypad 2 and Down Arrow
            { "kp-3", (0x5b, null) }, // Keypad 3 and PageDn
            { "kp-4", (0x5c, null) }, // Keypad 4 and Left Arrow
            { "kp-5", (0x5d, null) }, // Keypad 5
            { "kp-6", (0x5e, null) }, // Keypad 6 and Right Arrow
            { "kp-7", (0x5f, null) }, // Keypad 7 and Home
            { "kp-8", (0x60, null) }, // Keypad 8 and Up Arrow
            { "kp-9", (0x61, null) }, // Keypad 9 and Page Up
            { "kp-0", (0x62, null) }, // Keypad 0 and Insert
            { "kp-period", (0x63, null) }, // Keypad . and Delete
            // { "kp-stop", (0x63, null) },

            //{ "kp-backslash", (0x64, null) },  // Keyboard Non-US \ and |
            { "application", (0x65, null) }, // Keyboard Application
            { "compose", (0x65, null) },
            { "power", (0x66, null) }, // Keyboard Power
            { "kp-equals", (0x67, null) }, // Keypad =
            { "kp-equal", (0x67, null) },

            { "f13", (0x68, null) }, // Keyboard F13
            { "f14", (0x69, null) }, // Keyboard F14
            { "f15", (0x6a, null) }, // Keyboard F15
            { "f16", (0x6b, null) }, // Keyboard F16
            { "f17", (0x6c, null) }, // Keyboard F17
            { "f18", (0x6d, null) }, // Keyboard F18
            { "f19", (0x6e, null) }, // Keyboard F19
            { "f20", (0x6f, null) }, // Keyboard F20
            { "f21", (0x70, null) }, // Keyboard F21
            { "f22", (0x71, null) }, // Keyboard F22
            { "f23", (0x72, null) }, // Keyboard F23
            { "f24", (0x73, null) }, // Keyboard F24

            { "execute", (0x74, null) }, // Keyboard Execute
            { "open", (0x74, null) },
            { "help", (0x75, null) }, // Keyboard Help
            { "menu", (0x76, null) }, // Keyboard Menu
            { "props", (0x76, null) },
            { "select", (0x77, null) }, // Keyboard Select
            { "front", (0x77, null) },
            { "cancel", (0x78, null) }, // Keyboard Stop
            { "stop", (0x78, null) },
            { "redo", (0x79, null) }, // Keyboard Again
            { "again", (0x79, null) },
            { "undo", (0x7a, null) }, // Keyboard Undo
            { "cut", (0x7b, null) }, // Keyboard Cut
            { "copy", (0x7c, null) }, // Keyboard Copy
            { "paste", (0x7d, null) }, // Keyboard Paste
            { "find", (0x7e, null) }, // Keyboard Find
            { "mute", (0x7f, null) }, // Keyboard Mute

            { "volume-up", (0x80, null) }, // Keyboard Volume Up
            { "volume-down", (0x81, null) }, // Keyboard Volume Down

            { "media_play_pause", (0xe8, null) },
            { "media_stop", (0xe9, null) },
            { "media_prev", (0xea, null) },
            { "media_next", (0xeb, null) },
            { "media_eject", (0xec, null) },
            { "media_volume_up", (0xed, null) },
            { "media_volume_down", (0xee, null) },
            { "media_mute", (0xef, null) },
            { "browser_home", (0xf0, null) },
            { "browser_back", (0xf1, null) },
            { "browser_forward", (0xf2, null) },
            { "browser_stop", (0xf3, null) },
            { "browser_search", (0xf4, null) },
            { "browser_scrollup", (0xf5, null) },
            { "browser_scrolldown", (0xf6, null) },
            { "browser_edit", (0xf7, null) },
            { "browser_sleep", (0xf8, null) },
            { "browser_coffee", (0xf9, null) },
            { "browser_refresh", (0xfa, null) },
            { "browser_calc", (0xfb, null) },

            { "control", (0x00, KeyModifier.LeftCtrl) },
            { "ctrl", (0x00, KeyModifier.LeftCtrl) },
            { "lcontrol", (0x00, KeyModifier.LeftCtrl) },
            { "lctrl", (0x00, KeyModifier.LeftCtrl) },
            { "rcontrol", (0x00, KeyModifier.RightCtrl) },
            { "rctrl", (0x00, KeyModifier.RightCtrl) },

            { "alt", (0x00, KeyModifier.LeftAlt) },
            { "lalt", (0x00, KeyModifier.LeftAlt) },
            { "ralt", (0x00, KeyModifier.RightAlt) },

            { "shift", (0x00, KeyModifier.LeftShift) },
            { "lshift", (0x00, KeyModifier.LeftShift) },
            { "rshift", (0x00, KeyModifier.RightShift) },

            { "win", (0x00, KeyModifier.LeftMeta) },
            { "lwin", (0x00, KeyModifier.LeftMeta) },
            { "rwin", (0x00, KeyModifier.RightMeta) },
        };

    public static IDictionary<string, byte> Modifiers = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase) {
            { "left-ctrl", 0x01 },
            { "right-ctrl", 0x10 },
            { "left-shift", 0x02 },
            { "right-shift", 0x20 },
            { "left-alt", 0x04 },
            { "right-alt", 0x40 },
            { "left-meta", 0x08 },
            { "right-meta", 0x80 },
        };
  }
}