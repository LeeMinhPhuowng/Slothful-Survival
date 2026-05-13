using UnityEngine;
using UnityEngine.UI;
using Core.Foundation.MVVM.Demo.ViewModel;
using Core.Foundation.Reactive;
using R3;
using DisposableBag = Core.Foundation.Reactive.DisposableBag;

namespace Core.Foundation.MVVM.Demo.View
{

    /// <summary>
    /// View that subscribes to ViewModel streams and updates UI reactively.
    /// </summary>
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;

        private HealthViewModel _viewModel;
        private DisposableBag _subscriptions;

        public void Bind(HealthViewModel viewModel)
        {
            Unbind();
            _viewModel = viewModel;
            _subscriptions = new DisposableBag();

            _viewModel.HealthPercent
                .Subscribe(UpdateHealthBar)
                .AddTo(_subscriptions);
        }

        public void Unbind()
        {
            _subscriptions?.Dispose(); // Preventing memory leaks
            _subscriptions = null;
            _viewModel = null;
        }

        private void UpdateHealthBar(float percent)
        {
            if (healthSlider != null) healthSlider.value = percent;
        }

        // Called by UI Button.onClick
        public void OnDamageClicked()
        {
            _viewModel?.TakeDamage(10);
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
