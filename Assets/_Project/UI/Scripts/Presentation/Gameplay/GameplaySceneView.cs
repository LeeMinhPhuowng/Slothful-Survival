using Cysharp.Threading.Tasks;
using Game.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Gameplay
{
    public sealed class GameplaySceneView : UIView<GameplayViewModel>
    {
        [Header("Buttons")]
        [SerializeField] private Button pauseButton;

        private bool _isListening;

        public override void Bind(GameplayViewModel viewModel)
        {
            base.Bind(viewModel);
            AddListeners();
        }

        public override void Unbind()
        {
            RemoveListeners();
            base.Unbind();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            if (_isListening)
            {
                return;
            }

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
            }

            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveListener(OnPauseClicked);
            }

            _isListening = false;
        }

        private void OnPauseClicked()
        {
            ViewModel.OpenPause();
        }
    }
}
