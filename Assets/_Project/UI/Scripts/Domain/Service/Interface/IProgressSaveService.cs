using Game.UI.Save;

namespace Game.UI.Service
{
    public interface IProgressSaveService
    {
        bool HasSave();
        PlayerProgressData Load();
        void Save(PlayerProgressData data);
        void Delete();
    }
}
