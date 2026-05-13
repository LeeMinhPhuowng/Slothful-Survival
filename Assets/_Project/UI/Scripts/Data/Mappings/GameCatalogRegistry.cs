using System;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.Service;
using UnityEngine;

namespace Game.UI.Core
{
    public sealed class GameCatalogRegistry : MonoBehaviour, IGameCatalog
    {
        [SerializeField] private List<CharacterCatalogEntry> characters = new();
        [SerializeField] private List<MapCatalogEntry> maps = new();

        private readonly Dictionary<string, CharacterCatalogEntry> _charactersById = new();
        private readonly Dictionary<string, MapCatalogEntry> _mapsById = new();
        private bool _isBuilt;

        private void Awake()
        {
            Build();
        }

        public CharacterInfoSO GetCharacterConfig(string characterId)
        {
            EnsureBuilt();
            return _charactersById.TryGetValue(characterId, out CharacterCatalogEntry entry) ? entry.Config : null;
        }

        public LevelSO GetMapConfig(string mapId)
        {
            EnsureBuilt();
            return _mapsById.TryGetValue(mapId, out MapCatalogEntry entry) ? entry.Config : null;
        }

        public IReadOnlyList<CharacterInfoSO> GetAllCharacterConfigs()
        {
            EnsureBuilt();
            return _charactersById.Values.Select(entry => entry.Config).Where(config => config != null).ToArray();
        }

        public IReadOnlyList<LevelSO> GetAllMapConfigs()
        {
            EnsureBuilt();
            return _mapsById.Values.Select(entry => entry.Config).Where(config => config != null).ToArray();
        }

        public CharacterModel GetCharacter(string characterId)
        {
            EnsureBuilt();
            return _charactersById.TryGetValue(characterId, out CharacterCatalogEntry entry) ? entry.ToModel() : null;
        }

        public MapModel GetMap(string mapId)
        {
            EnsureBuilt();
            return _mapsById.TryGetValue(mapId, out MapCatalogEntry entry) ? entry.ToModel() : null;
        }

        public IReadOnlyList<CharacterModel> GetAllCharacters()
        {
            EnsureBuilt();
            return _charactersById.Values.Select(entry => entry.ToModel()).ToArray();
        }

        public IReadOnlyList<MapModel> GetAllMaps()
        {
            EnsureBuilt();
            return _mapsById.Values.Select(entry => entry.ToModel()).ToArray();
        }

        private void EnsureBuilt()
        {
            if (_isBuilt)
            {
                return;
            }

            Build();
        }

        private void Build()
        {
            _charactersById.Clear();
            _mapsById.Clear();

            foreach (CharacterCatalogEntry entry in characters)
            {
                if (entry == null || entry.Config == null)
                {
                    continue;
                }

                string characterId = entry.ResolveId();
                if (string.IsNullOrWhiteSpace(characterId) || _charactersById.ContainsKey(characterId))
                {
                    Debug.LogError($"[GameCatalogRegistry] Invalid or duplicate character id: {characterId}");
                    continue;
                }

                entry.CharacterId = characterId;
                _charactersById.Add(characterId, entry);
            }

            foreach (MapCatalogEntry entry in maps)
            {
                if (entry == null || entry.Config == null)
                {
                    continue;
                }

                string mapId = entry.ResolveId();
                if (string.IsNullOrWhiteSpace(mapId) || _mapsById.ContainsKey(mapId))
                {
                    Debug.LogError($"[GameCatalogRegistry] Invalid or duplicate map id: {mapId}");
                    continue;
                }

                entry.MapId = mapId;
                _mapsById.Add(mapId, entry);
            }

            _isBuilt = true;
        }
    }
}
