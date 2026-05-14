using Game.UI.Model;
using TMPro;
using UnityEngine;

namespace Game.UI.Presentation.Profile
{
    public sealed class CharacterPreviewView : MonoBehaviour
    {
        [SerializeField] private Animator previewAnimator;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Animator backgroundCharacterAnimator;

        public void Render(CharacterModel character)
        {
            if (nameText != null)
            {
                nameText.text = character?.DisplayName ?? string.Empty;
            }

            if (levelText != null)
            {
                levelText.text = character != null ? $"Lv.{character.Level}" : string.Empty;
            }

            if (previewAnimator != null && character != null)
            {
                previewAnimator.Play($"{character.DisplayName}Idle", 0, 0f);
            }

            if (backgroundCharacterAnimator != null && character != null)
            {
                backgroundCharacterAnimator.Play($"{character.DisplayName}Idle", 0, 0f);
            }
        }
    }
}
