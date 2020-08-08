namespace beholder_stalk
{
    using Newtonsoft.Json;

    public class ButtonPress : IJoystickAction
    {
        public ButtonPress()
        {
            Direction = PressDirection.PressAndRelease;
            Duration = new Duration() { Min = 40, Max = 141 };
        }

        [JsonProperty("button")]
        public string Button
        {
            get;
            set;
        }

        [JsonProperty("direction")]
        public PressDirection Direction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that, if specified, indicates the duration that the button will be pressed. Once the duration has expired the button will be released.
        /// </summary>
        [JsonProperty("duration")]
        public Duration Duration
        {
            get;
            set;
        }
    }
}
