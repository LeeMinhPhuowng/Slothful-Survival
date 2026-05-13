using System;

namespace Core.Foundation.MVVM
{

    /// <summary>
    /// Base class for ViewModels in the MVVM architecture.
    /// ViewModels expose state via R3 ReactiveProperty and own a DisposableBag
    /// for managing subscription lifetime. Per U6PHA §5.2 — no INotifyPropertyChanged.
    /// </summary>
    public abstract class BaseViewModel : IDisposable
    {
        /// <summary>
        /// Container that tracks disposables and disposes them when the ViewModel is disposed.
        /// </summary>
        protected Reactive.DisposableBag DisposableBag { get; } = new();

        /// <summary>
        /// Disposes the ViewModel and all managed subscriptions.
        /// </summary>
        public virtual void Dispose()
        {
            DisposableBag.Dispose();
        }
    }
}
