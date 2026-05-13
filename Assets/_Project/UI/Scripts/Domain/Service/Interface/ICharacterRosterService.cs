using System.Collections.Generic;
using Game.UI.Model;
using R3;

namespace Game.UI.Service
{
    public interface ICharacterRosterService
    {
        ReadOnlyReactiveProperty<CharacterModel> SelectedCharacter { get; }
        IReadOnlyList<CharacterModel> Characters { get; }
        CharacterModel GetSelectedCharacter();
        void SelectPrevious();
        void SelectNext();
        void Select(string characterId);
    }
}
