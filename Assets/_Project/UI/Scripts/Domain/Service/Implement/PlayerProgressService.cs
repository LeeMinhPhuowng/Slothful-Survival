using System;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.Save;
using R3;

namespace Game.UI.Service
{
    public sealed class PlayerProgressService : IPlayerProgressService, IDisposable
    {
        private readonly IProgressSaveService _saveService;
        private readonly bool _saveOnEveryChange;
        private readonly ReactiveProperty<float> _expPercent;
        private readonly ReactiveProperty<int> _currentLevel;
        private PlayerProgressData _data;

        public event Action Changed;

        public ReadOnlyReactiveProperty<float> ExpPercent => _expPercent;
        public ReadOnlyReactiveProperty<int> CurrentLevel => _currentLevel;
        public PlayerProgressData Data => _data;

        public PlayerProgressService(
            IProgressSaveService saveService,
            IGameCatalog gameCatalog,
            IEquipmentCatalog equipmentCatalog,
            int initialGold,
            int initialDiamond,
            bool resetProgressOnStart,
            bool saveOnEveryChange)
        {
            _saveService = saveService;
            _saveOnEveryChange = saveOnEveryChange;

            if (resetProgressOnStart)
            {
                _saveService.Delete();
            }

            bool hasExistingSave = _saveService.HasSave();
            _data = _saveService.Load() ?? CreateDefaultProgress(gameCatalog, equipmentCatalog, initialGold, initialDiamond);
            NormalizeProgress(gameCatalog, equipmentCatalog, !hasExistingSave);

            _expPercent = new ReactiveProperty<float>(_data.ExpPercent);
            _currentLevel = new ReactiveProperty<int>(_data.CurrentLevel);
            Persist();
        }

        public int GetGold()
        {
            return _data.Gold;
        }

        public int GetDiamond()
        {
            return _data.Diamond;
        }

        public void SetCurrency(CurrencyType currencyType, int amount)
        {
            if (currencyType == CurrencyType.Gold)
            {
                _data.Gold = Math.Max(0, amount);
            }
            else
            {
                _data.Diamond = Math.Max(0, amount);
            }

            Commit();
        }

        public bool IsEquipmentUnlocked(string itemId, bool defaultValue = false)
        {
            return ContainsId(_data.UnlockedEquipmentIds, itemId) || defaultValue;
        }

        public void SetEquipmentUnlocked(string itemId, bool isUnlocked)
        {
            SetIdState(_data.UnlockedEquipmentIds, itemId, isUnlocked);

            if (!isUnlocked)
            {
                _data.EquippedItems.RemoveAll(entry => entry != null && entry.ItemId == itemId);
            }

            Commit();
        }

        public string GetEquippedItemId(EquipmentSlot slot)
        {
            return _data.EquippedItems.FirstOrDefault(entry => entry != null && entry.Slot == slot)?.ItemId;
        }

        public void SetEquippedItem(EquipmentSlot slot, string itemId)
        {
            _data.EquippedItems.RemoveAll(entry => entry != null && entry.Slot == slot);

            if (!string.IsNullOrWhiteSpace(itemId))
            {
                _data.EquippedItems.Add(new EquipmentSlotSaveEntry
                {
                    Slot = slot,
                    ItemId = itemId
                });
            }

            Commit();
        }

        public bool IsCharacterUnlocked(string characterId, bool defaultValue = false)
        {
            return ContainsId(_data.UnlockedCharacterIds, characterId) || defaultValue;
        }

        public void SetCharacterUnlocked(string characterId, bool isUnlocked)
        {
            SetIdState(_data.UnlockedCharacterIds, characterId, isUnlocked);
            Commit();
        }

        public string GetSelectedCharacterId()
        {
            return _data.SelectedCharacterId;
        }

        public void SetSelectedCharacter(string characterId)
        {
            _data.SelectedCharacterId = characterId;
            Commit();
        }

        public bool IsMapUnlocked(string mapId, bool defaultValue = false)
        {
            return ContainsId(_data.UnlockedMapIds, mapId) || defaultValue;
        }

        public void SetMapUnlocked(string mapId, bool isUnlocked)
        {
            SetIdState(_data.UnlockedMapIds, mapId, isUnlocked);
            Commit();
        }

        public string GetSelectedMapId()
        {
            return _data.SelectedMapId;
        }

        public void SetSelectedMap(string mapId)
        {
            _data.SelectedMapId = mapId;
            Commit();
        }

        public void Save()
        {
            Persist();
        }

        public void ResetToDefaults(IGameCatalog gameCatalog, IEquipmentCatalog equipmentCatalog, int initialGold, int initialDiamond)
        {
            _data = CreateDefaultProgress(gameCatalog, equipmentCatalog, initialGold, initialDiamond);
            _expPercent.Value = _data.ExpPercent;
            _currentLevel.Value = _data.CurrentLevel;
            Commit();
        }

        public void Dispose()
        {
            Persist();
            _expPercent.Dispose();
            _currentLevel.Dispose();
        }

        private static PlayerProgressData CreateDefaultProgress(
            IGameCatalog gameCatalog,
            IEquipmentCatalog equipmentCatalog,
            int initialGold,
            int initialDiamond)
        {
            IReadOnlyList<CharacterModel> characters = gameCatalog.GetAllCharacters();
            IReadOnlyList<MapModel> maps = gameCatalog.GetAllMaps();
            IReadOnlyList<EquipmentItemModel> equipment = equipmentCatalog.GetAllItems();

            CharacterModel selectedCharacter = characters.FirstOrDefault(character => character.IsUnlocked) ?? characters.FirstOrDefault();
            MapModel selectedMap = maps.FirstOrDefault(map => map.IsUnlocked) ?? maps.FirstOrDefault();

            return new PlayerProgressData
            {
                Gold = initialGold,
                Diamond = initialDiamond,
                SelectedCharacterId = selectedCharacter?.CharacterId,
                SelectedMapId = selectedMap?.MapId,
                UnlockedCharacterIds = characters.Where(character => character.IsUnlocked).Select(character => character.CharacterId).ToList(),
                UnlockedMapIds = maps.Where(map => map.IsUnlocked).Select(map => map.MapId).ToList(),
                CompletedMapIds = maps.Where(map => map.IsCompleted).Select(map => map.MapId).ToList(),
                UnlockedEquipmentIds = equipment.Where(item => item.IsUnlocked).Select(item => item.ItemId).ToList()
            };
        }

        private void NormalizeProgress(IGameCatalog gameCatalog, IEquipmentCatalog equipmentCatalog, bool seedDefaultUnlocks)
        {
            EnsureLists();

            IReadOnlyList<CharacterModel> characters = gameCatalog.GetAllCharacters();
            IReadOnlyList<MapModel> maps = gameCatalog.GetAllMaps();

            if (seedDefaultUnlocks)
            {
                foreach (EquipmentItemModel item in equipmentCatalog.GetAllItems())
                {
                    if (item.IsUnlocked)
                    {
                        SetIdState(_data.UnlockedEquipmentIds, item.ItemId, true);
                    }
                }

                foreach (CharacterModel character in characters)
                {
                    if (character.IsUnlocked)
                    {
                        SetIdState(_data.UnlockedCharacterIds, character.CharacterId, true);
                    }
                }

                foreach (MapModel map in maps)
                {
                    if (map.IsUnlocked)
                    {
                        SetIdState(_data.UnlockedMapIds, map.MapId, true);
                    }

                    if (map.IsCompleted)
                    {
                        SetIdState(_data.CompletedMapIds, map.MapId, true);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(_data.SelectedCharacterId))
            {
                _data.SelectedCharacterId = characters.FirstOrDefault(character => IsCharacterUnlocked(character.CharacterId))?.CharacterId;
            }

            if (string.IsNullOrWhiteSpace(_data.SelectedMapId))
            {
                _data.SelectedMapId = maps.FirstOrDefault(map => IsMapUnlocked(map.MapId))?.MapId;
            }
        }

        private void EnsureLists()
        {
            _data ??= new PlayerProgressData();
            _data.UnlockedEquipmentIds ??= new List<string>();
            _data.EquippedItems ??= new List<EquipmentSlotSaveEntry>();
            _data.UnlockedCharacterIds ??= new List<string>();
            _data.UnlockedMapIds ??= new List<string>();
            _data.CompletedMapIds ??= new List<string>();
        }

        private void Commit()
        {
            Changed?.Invoke();

            if (_saveOnEveryChange)
            {
                Persist();
            }
        }

        private void Persist()
        {
            _data.ExpPercent = _expPercent?.Value ?? _data.ExpPercent;
            _data.CurrentLevel = _currentLevel?.Value ?? _data.CurrentLevel;
            _saveService.Save(_data);
        }

        private static bool ContainsId(List<string> ids, string id)
        {
            return !string.IsNullOrWhiteSpace(id) && ids.Any(item => item == id);
        }

        private static void SetIdState(List<string> ids, string id, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            ids.RemoveAll(item => item == id);

            if (enabled)
            {
                ids.Add(id);
            }
        }
    }
}
