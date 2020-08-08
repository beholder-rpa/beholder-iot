namespace beholder_stalk
{
    using Newtonsoft.Json;

    public class MouseTilt : IMouseAction
    {
        [JsonProperty("amount")]
        public sbyte Amount
        {
            get;
            set;
        }
    }
}
