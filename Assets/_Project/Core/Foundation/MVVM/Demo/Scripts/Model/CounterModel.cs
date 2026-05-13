namespace Core.Foundation.MVVM.Demo.Model
{

    /// <summary>
    /// Data model for Counter
    /// </summary>
    public class CounterModel
    {
        public int Count { get; set; }
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = int.MaxValue;
    }
}
