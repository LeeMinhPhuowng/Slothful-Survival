using System;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Model;
using R3;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class CharacterRosterService : ICharacterRosterService, IDisposable
    {
        private readonly IPlayerProgressService _progressService;
        private readonly ReactiveProperty<CharacterModel> _selectedCharacter;

        public ReadOnlyReactiveProperty<CharacterModel> SelectedCharacter => _selectedCharacter;
        public IReadOnlyList<CharacterModel> Characters { get; }

        public CharacterRosterService(IGameCatalog gameCatalog, IPlayerProgressService progressService)
        {
            _progressService = progressService;
            Characters = gameCatalog.GetAllCharacters();

            foreach (CharacterModel character in Characters)
            {
                character.IsUnlocked = _progressService.IsCharacterUnlocked(character.CharacterId, character.IsUnlocked);
            }

            string selectedCharacterId = _progressService.GetSelectedCharacterId();
            CharacterModel selectedCharacter = Characters.FirstOrDefault(character => character.CharacterId == selectedCharacterId && character.IsUnlocked)
                                               ?? Characters.FirstOrDefault(character => character.IsUnlocked)
                                               ?? Characters.FirstOrDefault();

            _selectedCharacter = new ReactiveProperty<CharacterModel>(selectedCharacter);
        }

        public void SelectPrevious()
        {
            SelectOffset(-1);
        }

        public CharacterModel GetSelectedCharacter()
        {
            return _selectedCharacter.Value;
        }

        public void SelectNext()
        {
            SelectOffset(1);
        }

        public void Select(string characterId)
        {
            CharacterModel character = Characters.FirstOrDefault(item => item.CharacterId == characterId);
            if (character != null && character.IsUnlocked)
            {
                _selectedCharacter.Value = character;
                _progressService.SetSelectedCharacter(character.CharacterId);
            }
        }

        public void Dispose()
        {
            _selectedCharacter.Dispose();
        }

        private void SelectOffset(int offset)
        {
            if (Characters.Count == 0 || _selectedCharacter.Value == null)
            {
                return;
            }

            int currentIndex = Math.Max(0, FindSelectedIndex());
            for (int i = 1; i <= Characters.Count; i++)
            {
                int nextIndex = (currentIndex + (offset * i) + Characters.Count) % Characters.Count;
                CharacterModel nextCharacter = Characters[nextIndex];
                if (!nextCharacter.IsUnlocked)
                {
                    continue;
                }

                _selectedCharacter.Value = nextCharacter;
                _progressService.SetSelectedCharacter(nextCharacter.CharacterId);
                return;
            }
        }

        private int FindSelectedIndex()
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                if (ReferenceEquals(Characters[i], _selectedCharacter.Value) || Characters[i].CharacterId == _selectedCharacter.Value.CharacterId)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
