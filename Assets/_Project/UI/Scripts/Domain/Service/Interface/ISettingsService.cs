using R3;

namespace Game.UI.Service
{
    public interface ISettingsService
    {
        ReactiveProperty<float> MusicVolume { get; }
        ReactiveProperty<float> SfxVolume { get; }
        void SetMusicVolume(float value);
        void SetSfxVolume(float value);
        void Save();
    }
}