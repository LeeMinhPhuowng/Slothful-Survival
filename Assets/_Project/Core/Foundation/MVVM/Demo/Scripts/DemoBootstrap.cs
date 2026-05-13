using Core.Foundation.MVVM.Demo.Model;
using Core.Foundation.MVVM.Demo.View;
using Core.Foundation.MVVM.Demo.ViewModel;
using UnityEngine;

namespace Core.Foundation.MVVM.Demo
{

    /// <summary>
    /// Composition root for MVVM demo scene.
    /// </summary>
    public class DemoBootstrap : MonoBehaviour
    {
        [SerializeField] private CounterView counterView;
        [SerializeField] private HealthView healthView;

        private CounterViewModel _counterVM;
        private HealthViewModel _healthVM;

        private void Awake()
        {
            CreateViewModels();
            BindViews();
        }

        private void OnDestroy()
        {
            UnbindViews();
            DisposeViewModels();
        }

        private void CreateViewModels()
        {
            _counterVM = new CounterViewModel(new CounterModel());
            _healthVM = new HealthViewModel(new HealthModel());
        }

        private void BindViews()
        {
            if(counterView != null) counterView.Bind(_counterVM);
            if(healthView != null) healthView.Bind(_healthVM);
        }

        private void UnbindViews()
        {
            if(counterView != null)  counterView.Unbind();
            if(healthView != null) healthView.Unbind();
        }
        private void DisposeViewModels()
        {
            _counterVM?.Dispose();
            _healthVM?.Dispose();
        }
    }
}
