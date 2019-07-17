namespace intro.Models
{
    public sealed class RecordTemperature
    {
        public RecordTemperature(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }
}