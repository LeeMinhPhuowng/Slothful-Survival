# Reactive Foundation Module

## Title & Summary

**Reactive Foundation** is a lightweight utility suite designed to enhance the use of the **R3 (Reactive Extensions for .NET)** library within Unity. It provides structured lifecycle management for subscriptions and a set of extension methods to simplify UI data binding and scene-safe observable operations.

---

## Dependencies

This module requires the following packages and scripts:

* **R3**: The core reactive library used for observables and properties.

---

## Key Classes

* **`DisposableBag`**: A container class (similar to `CompositeDisposable`) that collects multiple `IDisposable` objects and ensures they are all disposed of safely when the bag is disposed.
* **`DisposableExtensions`**: Provides the `.AddTo(DisposableBag)` syntax to easily chain subscription management.
* **`R3Extensions`**: A collection of static helpers for binding `ReactiveProperty` values to Unity UI components and creating scene-safe timers.

---

## Usage Example

### 1. UI Binding with DisposableBag

```csharp
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;
    
    private readonly DisposableBag _bag = new();
    private readonly ReactiveProperty<float> _health = new(100f);

    void Start()
    {
        // Bind health value to UI and add to the bag for cleanup
        _health.Select(x => x.ToString("F0"))
               .BindToText(healthText)
               .AddTo(_bag);

        _health.Select(x => x / 100f)
               .BindToFillAmount(healthBar)
               .AddTo(_bag);
    }

    void OnDestroy()
    {
        _bag.Dispose(); // Cleans up all subscriptions at once
    }
}

```

### 2. Scene-Safe Timer

```csharp
// Starts a timer that automatically stops if the current scene is unloaded
R3Extensions.SafeTimer(TimeSpan.FromSeconds(1))
    .Subscribe(tick => Debug.Log($"Tick: {tick}"))
    .AddTo(_bag);

```

---

## Architecture Notes

* **Lifecycle Management**: The `DisposableBag` implements a "dispose-on-add" pattern. If a disposable is added to a bag that has already been disposed, the item is immediately disposed of to prevent memory leaks and dangling subscriptions.
* **Thread Safety**: The UI binding helpers (`BindToText`, `BindToFillAmount`, etc.) utilize `.ObserveOnMainThread()`. This ensures that even if a reactive property is updated from a background thread, the UI update will always occur on the Unity Main Thread, preventing "Internal Unity Engine" errors.
* **Scene-Safe Observables**: The `SafeTimer` utility addresses a common Unity pitfall where background observables continue to run after a scene change. It uses a `sceneUnloadedTrigger` to automatically terminate the stream when the user navigates away from the scene where the timer was started.
* **Error Resilience**: The `Dispose` logic in `DisposableBag` wraps individual item disposal in a `try-catch` block, ensuring that one faulty disposal doesn't prevent the rest of the bag from being cleared.
* **Defensive Programming:** Extensions return `Disposable.Empty` if UI references are null, preventing crashes from unassigned Inspector fields.

## Best Practices

* **Always Dispose:** Ensure `_bag.Dispose()` is called in `OnDestroy()` or `OnDisable()`. Failing to do so results in "Orphaned Observables" that leak memory.
* **One Bag per Lifecycle:** Maintain one `DisposableBag` per object. Avoid static or shared bags to prevent accidental disposal of unrelated subscriptions.
* **Prefer SafeTimer:** Use `SafeTimer` instead of standard `Observable.Interval` for logic that should not survive a scene transition.
* **Minimize Side Effects:** Keep logic inside `.Subscribe()` focused. If you find yourself writing complex logic, consider moving it to a `.Select()` or a separate method before binding.

