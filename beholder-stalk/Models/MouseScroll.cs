namespace beholder_stalk
{
    using Newtonsoft.Json;

    public class MouseScroll : IMouseAction
    {
        [JsonProperty("amount")]
        public sbyte Amount
        {
            get;
            set;
        }
    }
}
