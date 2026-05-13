using Core.Foundation.MVVM.Demo.Model;
using Core.Foundation.Reactive;
using R3;

namespace Core.Foundation.MVVM.Demo.ViewModel
{

    /// <summary>
    /// ViewModel for Counter demo.
    /// </summary>
    public class CounterViewModel : BaseViewModel
    {
        private readonly CounterModel _model;
        private readonly ReactiveProperty<int> _count;

        public ReadOnlyReactiveProperty<int> Count { get; }

        public CounterViewModel(CounterModel model)
        {
            _model = model;
            _count = new ReactiveProperty<int>(_model.Count).AddTo(DisposableBag);
            Count = _count.ToReadOnlyReactiveProperty().AddTo(DisposableBag);
        }

        /// <summary>
        /// Increments counter within limits.
        /// </summary>
        public void Increment()
        {
            if (_count.Value >= _model.MaxValue) return;
            _count.Value++;
        }

        /// <summary>
        /// Decrements counter within limits.
        /// </summary>
        public void Decrement()
        {
            if (_count.Value <= _model.MinValue) return;
            _count.Value--;
        }
    }
}
