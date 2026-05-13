using Cysharp.Threading.Tasks;
using Game.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.MainMenu
{
    public sealed class MainMenuSceneView : UIView<MainMenuViewModel>
    {
        [Header("Buttons")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button chooseMapButton;

        private bool _isListening;

        public override void Bind(MainMenuViewModel viewModel)
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

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }

            if (shopButton != null)
            {
                shopButton.onClick.AddListener(OnShopClicked);
            }

            if (profileButton != null)
            {
                profileButton.onClick.AddListener(OnProfileClicked);
            }

            if (chooseMapButton != null)
            {
                chooseMapButton.onClick.AddListener(OnChooseMapClicked);
            }

            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(OnSettingsClicked);
            }

            if (shopButton != null)
            {
                shopButton.onClick.RemoveListener(OnShopClicked);
            }

            if (profileButton != null)
            {
                profileButton.onClick.RemoveListener(OnProfileClicked);
            }

            if (chooseMapButton != null)
            {
                chooseMapButton.onClick.RemoveListener(OnChooseMapClicked);
            }

            _isListening = false;
        }

        private void OnSettingsClicked()
        {
            ViewModel.OpenSettings();
        }

        private void OnShopClicked()
        {
            ViewModel.OpenShop();
        }

        private void OnProfileClicked()
        {
            ViewModel.OpenProfile();
        }

        private void OnChooseMapClicked()
        {
            ViewModel.ChooseMapAsync().Forget();
        }
    }
}
