namespace beholder_stalk
{
    using System;
    using System.Threading.Tasks;

    public static class Util
    {
        private static Random Random = new Random();

        public static int GetDelay(Duration duration = null)
        {

            if (duration != null)
            {
                if (duration.Delay.HasValue)
                {
                    return Math.Abs(duration.Delay.Value);
                }

                if (duration.Max.HasValue && duration.Min.HasValue)
                {
                    return Random.Next(Math.Abs(duration.Min.Value), Math.Abs(duration.Max.Value));
                }
            }

            return 0;
        }

        public static Task Hesitate(Duration duration = null)
        {
            return Task.Delay(GetDelay(duration));
        }
    }
}