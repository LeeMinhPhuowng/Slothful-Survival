using Cysharp.Threading.Tasks;
using Game.UI.Data;
using Game.UI.View;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.ChoosingMap
{
    public sealed class ChoosingMapSceneView : UIView<ChoosingMapViewModel>
    {
        [Header("Buttons")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button playButton;

        private bool _isListening;

        public override void Bind(ChoosingMapViewModel viewModel)
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

            if (returnButton != null)
            {
                returnButton.onClick.AddListener(OnReturnClicked);
            }

            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
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

            if (returnButton != null)
            {
                returnButton.onClick.RemoveListener(OnReturnClicked);
            }

            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayClicked);
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

        private void OnReturnClicked()
        {
            ViewModel.MainMenuAsync().Forget();
        }

        private void OnPlayClicked()
        {
            ViewModel.GameplayAsync().Forget();
        }
    }
}
