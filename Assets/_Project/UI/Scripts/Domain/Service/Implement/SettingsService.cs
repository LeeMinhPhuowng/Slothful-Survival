using R3;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class SettingsService : ISettingsService
    {
        private const string MusicVolumeKey = "settings_music_volume";
        private const string SfxVolumeKey = "settings_sfx_volume";

        public ReactiveProperty<float> MusicVolume { get; }
        public ReactiveProperty<float> SfxVolume { get; }

        public SettingsService()
        {
            MusicVolume = new ReactiveProperty<float>(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
            SfxVolume = new ReactiveProperty<float>(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));
            ApplyMusicVolume(MusicVolume.Value);
        }

        public void SetMusicVolume(float value)
        {
            float clamped = Mathf.Clamp01(value);
            MusicVolume.Value = clamped;
            ApplyMusicVolume(clamped);
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume.Value = Mathf.Clamp01(value);
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume.Value);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume.Value);
            PlayerPrefs.Save();
        }

        private static void ApplyMusicVolume(float value)
        {
            AudioListener.volume = Mathf.Clamp01(value);
        }
    }
}
