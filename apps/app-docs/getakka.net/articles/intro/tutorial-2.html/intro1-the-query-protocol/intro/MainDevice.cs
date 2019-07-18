namespace intro
{
    public class MainDevice
    {
        public sealed class ReadTemperature
        {
            public ReadTemperature(long requestId)
            {
                RequestId = requestId;
            }

            public long RequestId { get; }
        }
    }
}