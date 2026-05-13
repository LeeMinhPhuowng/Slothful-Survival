using System;
using Game.UI.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Profile
{
    public sealed class EquipmentTabButtonView : MonoBehaviour
    {
        [SerializeField] private EquipmentTab tab;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private GameObject selectedState;

        private Action<EquipmentTab> _clicked;
        private bool _isListening;

        public EquipmentTab Tab => tab;

        private void Awake()
        {
            NormalizeTabFromName();
        }

        public void Bind(Action<EquipmentTab> clicked)
        {
            _clicked = clicked;
            AddListeners();
        }

        public void Unbind()
        {
            RemoveListeners();
            _clicked = null;
        }

        public void Render(bool isSelected)
        {
            if (labelText != null)
            {
                labelText.text = GetDisplayName(tab);
            }

            if (selectedState != null)
            {
                selectedState.SetActive(isSelected);
            }
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void OnValidate()
        {
            NormalizeTabFromName();
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
            _clicked?.Invoke(tab);
        }

        private void NormalizeTabFromName()
        {
            if (TryResolveTab(gameObject.name, out EquipmentTab resolved))
            {
                tab = resolved;
            }
        }

        private static bool TryResolveTab(string source, out EquipmentTab resolved)
        {
            string normalized = source?.Trim().ToLowerInvariant() ?? string.Empty;

            if (normalized.Contains("all"))
            {
                resolved = EquipmentTab.All;
                return true;
            }

            if (normalized.Contains("helmet"))
            {
                resolved = EquipmentTab.Helmet;
                return true;
            }

            if (normalized.Contains("armor"))
            {
                resolved = EquipmentTab.Armor;
                return true;
            }

            if (normalized.Contains("gloves"))
            {
                resolved = EquipmentTab.Gloves;
                return true;
            }

            if (normalized.Contains("pants"))
            {
                resolved = EquipmentTab.Pants;
                return true;
            }

            if (normalized.Contains("boots"))
            {
                resolved = EquipmentTab.Boots;
                return true;
            }

            if (normalized.Contains("weapon"))
            {
                resolved = EquipmentTab.Weapon;
                return true;
            }

            resolved = default;
            return false;
        }

        private static string GetDisplayName(EquipmentTab value)
        {
            return value switch
            {
                EquipmentTab.All => "Tat ca",
                EquipmentTab.Armor => "Giap",
                EquipmentTab.Helmet => "Mu",
                EquipmentTab.Weapon => "Vu khi",
                EquipmentTab.Boots => "Giay",
                EquipmentTab.Pants => "Quan",
                EquipmentTab.Gloves => "Tay",
                _ => value.ToString()
            };
        }
    }
}
