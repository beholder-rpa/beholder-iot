namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Configuration;
  using System;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;
  using static beholder_stalk_v2.Protos.Keypress.Types;

  /// <summary>
  /// Represents a HID Keyboard
  /// </summary>
  public partial class Keyboard : HardwareInterfaceDevice
  {
    private static readonly object s_reportLock = new object();
    private static readonly byte[] s_currentReport = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    //Key Regex, based loosely off of https://www.autohotkey.com/docs/commands/Send.htm#keynames
    private static readonly Regex KeysRegex = new Regex(
        @"(?:(?<![\{])(?<Modifiers>[!+^#]*?)(?<Key>[^{!+^#])(?![^{!+^#]*?[\}])|(?:(?<KeyNameModifiers>[!+^#]*?)\{(?<KeyName>[^}]+?)(?:\s?(?:(?<Repeats>\d+)|(?<Direction>down|up)))?\}))"
        , RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public event EventHandler<KeyboardLedsChangedEventArgs> KeyboardLedsChanged;

    private readonly byte[] sizeBuffer = new byte[1];

    public Keyboard(IConfiguration config)
        : base(config["hid:keyboard:devPath"])
    {
      uint keyMin = 60;
      uint keyMax = 170;

      if (uint.TryParse(config["beholder_stalk_keymin"], out uint configKeyMin))
      {
        keyMin = configKeyMin;
      }

      if (uint.TryParse(config["beholder_stalk_keymax"], out uint configKeyMax))
      {
        keyMax = configKeyMax;
      }

      AverageKeypressDuration = new KeypressDuration() {  Min = keyMin, Max = keyMax };
      WatchNext();
    }

    public KeypressDuration AverageKeypressDuration
    {
      get;
      set;
    }

    private static byte? GetModifiers(KeyModifier? initial, IList<string> mods)
    {
      byte? modifierBytes = null;
      if (initial.HasValue)
      {
        modifierBytes = (byte)initial.Value;
      }

      foreach (var key in mods)
      {
        if (string.IsNullOrWhiteSpace(key))
        {
          continue;
        }

        var lowerKey = key.ToLowerInvariant();
        if (Modifiers.ContainsKey(lowerKey))
        {
          modifierBytes = (byte)(modifierBytes | Modifiers[lowerKey]);
        }
      }

      return modifierBytes;
    }

    private static bool IsPressed(byte key, out int pressedKeyIndex)
    {
      for (int i = 2; i < s_currentReport.Length; i++)
      {
        if (s_currentReport[i] == key)
        {
          pressedKeyIndex = i;
          return true;
        }
      }

      pressedKeyIndex = -1;
      return false;
    }

    private void SendCurrentReport()
    {
      Stream.Write(s_currentReport);
      Stream.Flush();
    }

    private int SendKeyPress(byte? key, byte? modifiers)
    {
      int ix = -1;

      lock (s_reportLock)
      {
        // Always set the modifier if it has a value.
        if (modifiers.HasValue)
        {
          s_currentReport[0] = (byte)(s_currentReport[0] | modifiers.Value);
        }

        // If key is specified, and isn't pressed, find a free spot and set the value.
        if (key.HasValue && !IsPressed(key.Value, out ix))
        {
          for (int i = 2; i < 8; i++)
          {
            if (s_currentReport[i] == 0x00)
            {
              ix = i;
              break;
            }
          }

          // Update the key at the index.
          if (ix > 1)
          {
            s_currentReport[ix] = key.Value;
          }
        }

        SendCurrentReport();
        return ix;
      }
    }

    private void SendKeyRelease(byte? key, byte? modifiers)
    {
      lock (s_reportLock)
      {
        // Always set the modifier if it has a value.
        if (modifiers.HasValue)
        {
          s_currentReport[0] = (byte)(s_currentReport[0] ^ modifiers.Value);
        }

        if (key.HasValue && IsPressed(key.Value, out int ix))
        {
          s_currentReport[ix] = 0x00;
        }

        SendCurrentReport();
      }
    }

    protected void OnKeyboardLedChanged(KeyboardLedsChangedEventArgs e)
    {
      KeyboardLedsChanged?.Invoke(this, e);
    }

    protected void WatchNext()
    {
      Stream.BeginRead(sizeBuffer, 0, 1, new AsyncCallback(ReadCallback), null);
    }

    private void ReadCallback(IAsyncResult ar)
    {
      int bytesRead = Stream.EndRead(ar);
      if (bytesRead == 0)
      {
        return;
      }

      var leds = (KeyboardLeds)sizeBuffer[0];
      OnKeyboardLedChanged(new KeyboardLedsChangedEventArgs(leds));
      WatchNext();
    }

    /// <summary>
    /// Sends the specified key, optionally indicating modifiers and duration.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="modifiers"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async Task SendKey(string key, KeyDirection direction = KeyDirection.PressAndRelease, IList<string> modifiers = null, KeypressDuration duration = null)
    {
      if (string.IsNullOrEmpty(key))
      {
        return;
      }

      if (duration == null)
      {
        duration = AverageKeypressDuration;
      }

      // KeyNames of length == 1 are case-sensitive.
      if (key.Length > 1)
      {
        key = key.ToLowerInvariant();
      }

      if (!Keys.ContainsKey(key))
      {
        return;
      }

      var keyValue = Keys[key];
      var modifiersValue = GetModifiers(keyValue.Item2, modifiers);

      switch (direction)
      {
        case KeyDirection.Press:
          SendKeyPress(keyValue.Item1, modifiersValue);
          break;
        case KeyDirection.Release:
          SendKeyRelease(keyValue.Item1, modifiersValue);
          break;
        case KeyDirection.PressAndRelease:
        default:
          SendKeyPress(keyValue.Item1, modifiersValue);
          await Util.Think(duration);
          SendKeyRelease(keyValue.Item1, modifiersValue);
          break;
      }
    }

    public Task SendKey(Keypress keypress)
    {
      return SendKey(keypress.Key, keypress.KeyDirection, keypress.Modifiers, keypress.Duration);
    }

    public async Task SendKeys(string keys)
    {
      static IList<string> GetModifiers(Match m, string groupName = "Modifiers")
      {
        var modifiers = new List<string>();
        if (m.Groups.ContainsKey(groupName))
        {
          foreach (var c in m.Groups[groupName].Value)
          {
            switch (c)
            {
              case '!':
                modifiers.Add("Left-Alt");
                break;
              case '+':
                modifiers.Add("Left-Shift");
                break;
              case '^':
                modifiers.Add("Left-Ctrl");
                break;
              case '#':
                modifiers.Add("Left-Meta");
                break;
            }
          }
        }

        return modifiers;
      }

      var keypresses = new List<Keypress>();
      foreach (Match m in KeysRegex.Matches(keys))
      {
        Keypress keypress = null;
        // This must be null or empty as whitespace is valid.
        if (m.Groups.ContainsKey("Key") && !string.IsNullOrEmpty(m.Groups["Key"].Value))
        {
          var key = m.Groups["Key"].Value;

          keypress = new Keypress()
          {
            Key = key,
            KeyDirection = KeyDirection.PressAndRelease,
            Duration = AverageKeypressDuration,
          };

          keypress.Modifiers.AddRange(GetModifiers(m));
        }
        else if (m.Groups.ContainsKey("KeyName") && !string.IsNullOrWhiteSpace(m.Groups["KeyName"].Value))
        {
          var keyName = m.Groups["KeyName"].Value;
          keypress = new Keypress()
          {
            Key = keyName,
            Duration = AverageKeypressDuration
          };
          keypress.Modifiers.AddRange(GetModifiers(m, "KeyNameModifiers"));
        }

        if (keypress != null)
        {
          if (m.Groups.ContainsKey("Direction"))
          {
            switch (m.Groups["Direction"].Value.ToLowerInvariant())
            {
              case "press":
              case "down":
                keypress.KeyDirection = KeyDirection.Press;
                keypress.Duration = KeypressDuration.Infinite;
                break;
              case "release":
              case "up":
                keypress.KeyDirection = KeyDirection.Release;
                keypress.Duration = KeypressDuration.Infinite;
                break;
              default:
                keypress.KeyDirection = KeyDirection.PressAndRelease;
                keypress.Duration = AverageKeypressDuration;
                break;
            }
          }

          if (m.Groups.ContainsKey("Repeats") && int.TryParse(m.Groups["Repeats"].Value, out int repeats))
          {
            for (int i = 0; i < Math.Abs(repeats); i++)
            {
              keypresses.Add(keypress);
            }
          }
          else
          {
            keypresses.Add(keypress);
          }
        }
      }

      // Now that we have the keypresses, press them.
      foreach (var keypress in keypresses)
      {
        await SendKey(keypress);
      }
    }

    public override void SendRaw(byte[] report)
    {
      if (report.Length != s_currentReport.Length)
      {
        return;
      }

      lock (s_currentReport)
      {
        for (int i = 0; i < s_currentReport.Length; i++)
        {
          s_currentReport[i] = report[i];
        }

        SendCurrentReport();
      }
    }

    public void SendKeysReset()
    {
      lock (s_reportLock)
      {
        // Reset the current report to all blanks.
        for (int i = 0; i < s_currentReport.Length; i++)
        {
          s_currentReport[i] = 0x00;
        }

        SendCurrentReport();
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        SendKeysReset();
      }

      base.Dispose(disposing);
    }
  }
}