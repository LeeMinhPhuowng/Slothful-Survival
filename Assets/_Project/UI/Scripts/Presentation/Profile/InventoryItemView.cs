using System;
using Game.UI.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Profile
{
    public sealed class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image rarityBackgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private GameObject equippedState;
        [SerializeField] private GameObject lockedState;
        [SerializeField] private RarityBackgroundEntry[] rarityBackgrounds;

        private EquipmentItemModel _item;
        private Action<string> _clicked;
        private bool _isListening;

        public void Bind(Action<string> clicked)
        {
            _clicked = clicked;
            AddListeners();
        }

        public void Unbind()
        {
            RemoveListeners();
            _clicked = null;
            _item = null;
        }

        public void Render(EquipmentItemModel item, bool isEquipped)
        {
            _item = item;

            if (nameText != null)
            {
                nameText.text = item?.DisplayName ?? string.Empty;
            }

            if (levelText != null)
            {
                levelText.text = item != null ? $"Lv.{item.Level}" : string.Empty;
            }

            if (rarityText != null)
            {
                rarityText.text = item != null ? item.Rarity.ToString() : string.Empty;
            }

            if (rarityBackgroundImage != null)
            {
                Sprite background = item != null ? RarityBackgroundResolver.Resolve(rarityBackgrounds, item.Rarity) : null;
                rarityBackgroundImage.sprite = background;
                rarityBackgroundImage.enabled = background != null;
            }

            if (iconImage != null)
            {
                iconImage.sprite = item?.Icon;
                iconImage.enabled = item?.Icon != null;
            }

            if (equippedState != null)
            {
                equippedState.SetActive(isEquipped);
            }

            if (lockedState != null)
            {
                lockedState.SetActive(item != null && !item.IsUnlocked);
            }
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            if (_isListening || button == null)
            {
                return;
            }

            button.onClick.AddListener(OnClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening || button == null)
            {
                return;
            }

            button.onClick.RemoveListener(OnClicked);
            _isListening = false;
        }

        private void OnClicked()
        {
            if (_item == null)
            {
                return;
            }
            
            _clicked?.Invoke(_item.ItemId);
        }
    }
}
