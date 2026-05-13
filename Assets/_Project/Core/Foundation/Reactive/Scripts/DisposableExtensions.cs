using System;

namespace Core.Foundation.Reactive
{

    public static class DisposableExtensions
    {
        public static T AddTo<T>(this T disposable, DisposableBag bag) where T : IDisposable
        {
            if (bag == null)
            {
                throw new ArgumentNullException(nameof(bag), "DisposableBag cannot be null.");
            }
            bag.Add(disposable);
            return disposable;
        }
    }
}
