using System;
using Game.UI.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Profile
{
    public sealed class ItemDetailView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text comparisonText;
        [SerializeField] private TMP_Text actionButtonText;
        [SerializeField] private Button actionButton;
        [SerializeField] private GameObject emptyState;
        [SerializeField] private GameObject contentRoot;

        private Action _actionClicked;
        private bool _isListening;

        public void Bind(Action actionClicked)
        {
            _actionClicked = actionClicked;
            AddListeners();
        }

        public void Unbind()
        {
            RemoveListeners();
            _actionClicked = null;
        }

        public void Render(EquipmentItemModel selectedItem, EquipmentItemModel comparedItem, bool isEquipped)
        {
            bool hasItem = selectedItem != null;

            if (emptyState != null)
            {
                emptyState.SetActive(!hasItem);
            }

            if (contentRoot != null)
            {
                contentRoot.SetActive(hasItem);
            }

            if (!hasItem)
            {
                return;
            }

            if (iconImage != null)
            {
                iconImage.sprite = selectedItem.Icon;
                iconImage.enabled = selectedItem.Icon != null;
            }

            if (nameText != null)
            {
                nameText.text = selectedItem.DisplayName;
            }

            if (rarityText != null)
            {
                rarityText.text = selectedItem.Rarity.ToString();
            }

            if (descriptionText != null)
            {
                descriptionText.text = selectedItem.Description;
            }

            if (statsText != null)
            {
                statsText.text = BuildStatsText(selectedItem);
            }

            if (comparisonText != null)
            {
                comparisonText.text = BuildComparisonText(selectedItem, comparedItem);
            }

            if (actionButtonText != null)
            {
                actionButtonText.text = isEquipped ? "Thao" : comparedItem == null ? "Lap" : "Thay the";
            }

            if (actionButton != null)
            {
                actionButton.interactable = true;
            }
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            if (_isListening || actionButton == null)
            {
                return;
            }

            actionButton.onClick.AddListener(OnActionClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening || actionButton == null)
            {
                return;
            }

            actionButton.onClick.RemoveListener(OnActionClicked);
            _isListening = false;
        }

        private void OnActionClicked()
        {
            _actionClicked?.Invoke();
        }

        private static string BuildStatsText(EquipmentItemModel item)
        {
            return $"ATK +{item.Damage}\nDEF +{item.Armor}\nHP +{item.MaxHealth}\nSPD {FormatSigned(item.MoveSpeed)}";
        }

        private static string BuildComparisonText(EquipmentItemModel selected, EquipmentItemModel compared)
        {
            if (compared == null || compared.ItemId == selected.ItemId)
            {
                return "Chua co item khac trong slot nay.";
            }

            return $"So voi {compared.DisplayName}\n"
                   + $"ATK {FormatSigned(selected.Damage - compared.Damage)}\n"
                   + $"DEF {FormatSigned(selected.Armor - compared.Armor)}\n"
                   + $"HP {FormatSigned(selected.MaxHealth - compared.MaxHealth)}\n"
                   + $"SPD {FormatSigned(selected.MoveSpeed - compared.MoveSpeed)}";
        }

        private static string FormatSigned(int value)
        {
            return value >= 0 ? $"+{value}" : value.ToString();
        }
    }
}
