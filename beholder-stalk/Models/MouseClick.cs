namespace beholder_stalk
{
    using Newtonsoft.Json;

    public class MouseClick : IMouseAction
    {
        public MouseClick()
        {
            Direction = ClickDirection.PressAndRelease;
            Duration = new Duration() { Min = 40, Max = 141 };
        }

        [JsonProperty("button")]
        public string Button
        {
            get;
            set;
        }

        [JsonProperty("direction")]
        public ClickDirection Direction
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
