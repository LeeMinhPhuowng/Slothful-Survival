using System;
using Core.Foundation.MVVM.Demo.ViewModel;
using R3;
using TMPro;
using UnityEngine;

namespace Core.Foundation.MVVM.Demo.View
{

    /// <summary>
    /// Passive View responsible only for rendering Counter UI and forwarding user input.
    /// ViewModel lifecycle is owned externally (e.g., Bootstrap).
    /// </summary>
    public class CounterView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text counterText;

        private CounterViewModel _viewModel;
        private IDisposable _binding;

        /// <summary>
        /// Binds a ViewModel instance to this View.
        /// </summary>
        public void Bind(CounterViewModel viewModel)
        {
            Unbind();

            _viewModel = viewModel;
            _binding = _viewModel.Count.Subscribe(value => counterText.text = value.ToString());
        }

        /// <summary>
        /// Unbinds the current ViewModel from this View.
        /// </summary>
        public void Unbind()
        {
            _binding?.Dispose();
            _binding = null;
            _viewModel = null;
        }

        /// <summary>
        /// UI command entry point for incrementing the counter.
        /// </summary>
        public void OnIncrementClicked()
        {
            _viewModel?.Increment();
        }

        /// <summary>
        /// UI command entry point for decrementing the counter.
        /// </summary>
        public void OnDecrementClicked()
        {
            _viewModel?.Decrement();
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
