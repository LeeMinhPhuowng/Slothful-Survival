# BaseViewModel - MVVM Foundation

A minimal base class for Unity ViewModels providing property change notification and subscription lifecycle management.

## 1. Dependencies

- **System.ComponentModel.INotifyPropertyChanged** - Property change notification
- **R3** (Reactive Extensions) - Observable streams and reactive properties
- **Core.Foundation.Reactive.DisposableBag** - Subscription lifecycle management

## 2. Main Classes

### BaseViewModel (Core)

Abstract base class providing:

```csharp
public abstract class BaseViewModel : IDisposable, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected DisposableBag DisposableBag { get; } = new();
    
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null);
    protected virtual void OnPropertyChanged(string propertyName);
    public virtual void Dispose();
}
```

**Key Capabilities:**
- `SetProperty<T>` - Updates field only when value changes, then raises PropertyChanged
- `DisposableBag` - Tracks reactive subscriptions for cleanup
- `Dispose()` - Cleans up all tracked subscriptions

### CounterViewModel (Pattern A: Classic MVVM)

Demonstrates basic property notification without reactive streams.

```csharp
public class CounterViewModel : BaseViewModel
{
    private int _count;
    public int Count
    {
        get => _count;
        private set => SetProperty(ref _count, value);
    }
    
    public void Increment() { /* ... */ }
    public void Decrement() { /* ... */ }
}
```

**Use Case:** Simple, isolated UI components with no external stream dependencies.

### HealthViewModel (Pattern B: Reactive Standard) **[PROJECT PRIORITY]**

Official coding standard using R3 integration.

```csharp
public class HealthViewModel : BaseViewModel
{
    private readonly ReactiveProperty<int> _currentHealth;
    public ReadOnlyReactiveProperty<int> CurrentHealth { get; }
    public ReadOnlyReactiveProperty<float> HealthPercent { get; }
    
    public HealthViewModel(HealthModel model)
    {
        _currentHealth = new ReactiveProperty<int>(model.CurrentHealth).AddTo(DisposableBag);
        CurrentHealth = _currentHealth.ToReadOnlyReactiveProperty().AddTo(DisposableBag);
        
        HealthPercent = CurrentHealth
            .Select(h => MaxHealth > 0 ? (float)h / MaxHealth : 0f)
            .ToReadOnlyReactiveProperty().AddTo(DisposableBag);
    }
}
```

**Use Case:** All major features (Combat, Economy, Gameplay), complex state derivation, async handling.

## 3. Usage Examples

### Classic MVVM Binding (Pattern A)

**View:**
```csharp
public class CounterView : MonoBehaviour
{
    private CounterViewModel _viewModel;
    
    public void Bind(CounterViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        Refresh();
    }
    
    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CounterViewModel.Count))
            Refresh();
    }
    
    public void Unbind()
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }
}
```

### Reactive Binding (Pattern B)

**View:**
```csharp
public class HealthView : MonoBehaviour
{
    private HealthViewModel _viewModel;
    private DisposableBag _subscriptions;
    
    public void Bind(HealthViewModel viewModel)
    {
        _viewModel = viewModel;
        _subscriptions = new DisposableBag();
        
        _viewModel.HealthPercent
            .Subscribe(UpdateHealthBar)
            .AddTo(_subscriptions);
    }
    
    public void Unbind()
    {
        _subscriptions?.Dispose();
    }
}
```

### Lifecycle Management

**Bootstrap (Composition Root):**
```csharp
public class DemoBootstrap : MonoBehaviour
{
    private CounterViewModel _counterVM;
    private HealthViewModel _healthVM;
    
    private void Awake()
    {
        _counterVM = new CounterViewModel(new CounterModel());
        _healthVM = new HealthViewModel(new HealthModel());
        
        counterView.Bind(_counterVM);
        healthView.Bind(_healthVM);
    }
    
    private void OnDestroy()
    {
        counterView.Unbind();
        healthView.Unbind();
        
        _counterVM?.Dispose();
        _healthVM?.Dispose();
    }
}
```

## 4. Architecture Notes

### Pattern Selection

**Pattern A (Classic MVVM):**
- **When:** Lightweight, non-reactive scenarios with simple state
- **How:** Use `SetProperty<T>` and `INotifyPropertyChanged`
- **Status:** Legacy support only, secondary to Reactive Standard

**Pattern B (Reactive Standard) [RECOMMENDED]:**
- **When:** All new features, complex state, async operations
- **How:** Use R3 (`ReactiveProperty`, `Observable`) + `DisposableBag`
- **Status:** Official project standard for Fruit Fortress (U6PHA)

### Integration Rules

1. **Default Choice:** Always use Pattern B (R3) for new features
2. **Subscription Management:** All reactive subscriptions MUST use `.AddTo(DisposableBag)`
3. **Disposal:** ViewModels MUST be disposed by their owner (Bootstrap/Factory)
4. **Memory Safety:** Views MUST unbind subscriptions in `OnDestroy()`

### Best Practices

```csharp
// Reactive property with disposal
_health = new ReactiveProperty<int>(100).AddTo(DisposableBag);

// Read-only exposure prevents external mutation
public ReadOnlyReactiveProperty<int> Health { get; }

// Derived state with disposal
HealthPercent = _health
    .Select(h => (float)h / MaxHealth)
    .ToReadOnlyReactiveProperty()
    .AddTo(DisposableBag);

// View subscription with cleanup
_viewModel.Health
    .Subscribe(OnHealthChanged)
    .AddTo(_subscriptions);
```

---

**Note:** Both patterns share the same disposal lifecycle through `BaseViewModel.Dispose()`, ensuring proper cleanup regardless of implementation approach.