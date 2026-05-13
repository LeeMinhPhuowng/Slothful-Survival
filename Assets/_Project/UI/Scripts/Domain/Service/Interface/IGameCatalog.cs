using System.Collections.Generic;
using Game.UI.Model;

namespace Game.UI.Service
{
    public interface IGameCatalog
    {
        CharacterInfoSO GetCharacterConfig(string characterId);
        LevelSO GetMapConfig(string mapId);
        IReadOnlyList<CharacterInfoSO> GetAllCharacterConfigs();
        IReadOnlyList<LevelSO> GetAllMapConfigs();
        CharacterModel GetCharacter(string characterId);
        MapModel GetMap(string mapId);
        IReadOnlyList<CharacterModel> GetAllCharacters();
        IReadOnlyList<MapModel> GetAllMaps();
    }
}