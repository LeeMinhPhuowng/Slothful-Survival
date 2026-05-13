using System;
using R3;
using Core.Foundation.MVVM.Demo.Model;
using Core.Foundation.Reactive;

namespace Core.Foundation.MVVM.Demo.ViewModel
{

    /// <summary>
    /// Reactive ViewModel exposing state as observable streams.
    /// Demonstrates R3 integration.
    /// </summary>
    public class HealthViewModel : BaseViewModel
    {
        private readonly HealthModel _model;
        private readonly ReactiveProperty<int> _currentHealth;

        // Exposed as ReadOnly to prevent external mutation of the stream
        public ReadOnlyReactiveProperty<int> CurrentHealth { get; }
        public ReadOnlyReactiveProperty<float> HealthPercent { get; }
        public int MaxHealth => _model.MaxHealth;

        public HealthViewModel(HealthModel model)
        {
            _model = model;
            
            // Initialize reactive properties with current model state
            _currentHealth = new ReactiveProperty<int>(_model.CurrentHealth).AddTo(DisposableBag);
            CurrentHealth = _currentHealth.ToReadOnlyReactiveProperty().AddTo(DisposableBag);
            
            // Derived state 
            HealthPercent = CurrentHealth
                .Select(h => MaxHealth > 0 ? (float)h / MaxHealth : 0f)
                .ToReadOnlyReactiveProperty().AddTo(DisposableBag);
        }

        public void TakeDamage(int amount)
        {
            _currentHealth.Value = Math.Max(0, _currentHealth.Value - amount);
        }
    }
}
