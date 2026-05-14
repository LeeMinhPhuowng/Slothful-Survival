using Game.UI.Data;
using Game.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Settings
{
    public sealed class SettingsPanelView : UIPanelView
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button closeButton;

        private SettingsPanelViewModel _viewModel;
        private bool _isListening;

        public override PanelId PanelId => PanelId.Settings;

        public void Bind(SettingsPanelViewModel viewModel)
        {
            _viewModel = viewModel;
            Refresh();
            AddListeners();
        }

        public void Unbind()
        {
            RemoveListeners();
            _viewModel = null;
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Refresh()
        {
            if (_viewModel == null)
            {
                return;
            }

            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(_viewModel.MusicVolume);
            }

            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(_viewModel.SfxVolume);
            }
        }

        private void AddListeners()
        {
            if (_isListening)
            {
                return;
            }

            musicSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxSlider?.onValueChanged.AddListener(OnSfxVolumeChanged);
            closeButton?.onClick.AddListener(OnCloseClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            musicSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            sfxSlider?.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            closeButton?.onClick.RemoveListener(OnCloseClicked);
            _isListening = false;
        }

        private void OnMusicVolumeChanged(float value)
        {
            _viewModel?.SetMusicVolume(value);
        }

        private void OnSfxVolumeChanged(float value)
        {
            _viewModel?.SetSfxVolume(value);
        }

        private void OnCloseClicked()
        {
            _viewModel?.Close();
        }
    }
}
