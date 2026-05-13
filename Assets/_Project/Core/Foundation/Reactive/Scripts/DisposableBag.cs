using System;
using System.Collections.Generic;

namespace Core.Foundation.Reactive
{

    /// <summary>
    /// A collection of disposable objects that manages their lifecycle together.
    /// </summary>
    public class DisposableBag : IDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        private bool _isDisposed = false;

        /// <summary>
        /// Adds a disposable object to the bag.
        /// </summary>
        public void Add(IDisposable disposable)
        {
            if (disposable == null) return;

            if (_isDisposed)
            {
                disposable.Dispose();
                return;
            }
            
            _disposables.Add(disposable);
        }

        /// <summary>
        /// Disposes all tracked disposable objects and clears the bag.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            int count = _disposables.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    _disposables[i]?.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing item: {ex}");
                }
            }

            _disposables.Clear();
        }
    }
}
