namespace beholder_stalk
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a press and optionally, release, of a particular key and its modifiers
    /// </summary>
    public class Keypress
    {
        public Keypress()
        {
            Direction = KeyDirection.PressAndRelease;
            Duration = new Duration() { Min = 80, Max = 251 };
        }

        [JsonProperty("key")]
        public string Key
        {
            get;
            set;
        }

        [JsonProperty("direction")]
        public KeyDirection Direction
        {
            get;
            set;
        }

        [JsonProperty("modifiers")]
        public IList<string> Modifiers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that, if specified, indicates the duration that the key will be pressed. Once the duration has expired the key will be released.
        /// </summary>
        [JsonProperty("duration")]
        public Duration Duration
        {
            get;
            set;
        }
    }
}
