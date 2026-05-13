using System;
using Game.UI.Data;
using Game.UI.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Profile
{
    public sealed class EquipmentSlotView : MonoBehaviour
    {
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private Button button;
        [SerializeField] private Image rarityBackgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private GameObject emptyState;
        [SerializeField] private GameObject selectedState;
        [SerializeField] private GameObject compatibleState;
        [SerializeField] private RarityBackgroundEntry[] rarityBackgrounds;

        private Action<EquipmentSlot> _clicked;
        private bool _isListening;

        public EquipmentSlot Slot => slot;

        public void Bind(Action<EquipmentSlot> clicked)
        {
            _clicked = clicked;
            AddListeners();
        }

        public void Unbind()
        {
            RemoveListeners();
            _clicked = null;
        }

        public void Render(EquipmentItemModel item, bool isSelected, bool isCompatible)
        {
            if (labelText != null)
            {
                labelText.text = item != null ? item.DisplayName : GetDisplayName(slot);
            }

            if (iconImage != null)
            {
                iconImage.sprite = item?.Icon;
                iconImage.enabled = item?.Icon != null;
            }

            if (rarityBackgroundImage != null)
            {
                Sprite background = item != null ? RarityBackgroundResolver.Resolve(rarityBackgrounds, item.Rarity) : null;
                rarityBackgroundImage.sprite = background;
                rarityBackgroundImage.enabled = background != null;
            }

            if (emptyState != null)
            {
                emptyState.SetActive(item == null);
            }

            if (selectedState != null)
            {
                selectedState.SetActive(isSelected);
            }

            if (compatibleState != null)
            {
                compatibleState.SetActive(isCompatible);
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
            _clicked?.Invoke(slot);
        }

        private static string GetDisplayName(EquipmentSlot value)
        {
            return value switch
            {
                EquipmentSlot.Armor => "Giap",
                EquipmentSlot.Helmet => "Mu",
                EquipmentSlot.Weapon => "Vu khi",
                EquipmentSlot.Boots => "Giay",
                EquipmentSlot.Pants => "Quan",
                EquipmentSlot.Gloves => "Tay",
                _ => value.ToString()
            };
        }
    }
}
