using Game.UI.Data;
using Game.UI.Service;

namespace Game.UI.Presentation.Settings
{
    public sealed class SettingsPanelViewModel
    {
        private readonly IPanelService _panelService;
        private readonly ISettingsService _settingsService;

        public SettingsPanelViewModel(IPanelService panelService, ISettingsService settingsService)
        {
            _panelService = panelService;
            _settingsService = settingsService;
        }

        public float MusicVolume => _settingsService.MusicVolume.Value;
        public float SfxVolume => _settingsService.SfxVolume.Value;

        public void SetMusicVolume(float value)
        {
            _settingsService.SetMusicVolume(value);
        }

        public void SetSfxVolume(float value)
        {
            _settingsService.SetSfxVolume(value);
        }

        public void Close()
        {
            _settingsService.Save();
            _panelService.Close(PanelId.Settings);
        }
    }
}
