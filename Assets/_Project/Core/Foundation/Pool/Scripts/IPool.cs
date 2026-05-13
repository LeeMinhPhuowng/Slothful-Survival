namespace Core.Foundation.Pool
{

    public interface IPool<T> where T : class
    {
        T Get();
        void Release(T item);
        int ActiveCount { get; }
        int AvailableCount { get; }
    }
}
