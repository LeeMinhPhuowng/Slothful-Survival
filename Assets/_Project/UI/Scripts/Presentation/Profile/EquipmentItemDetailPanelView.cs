using Game.UI.Data;
using Game.UI.Model;
using Game.UI.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Profile
{
    public sealed class EquipmentItemDetailPanelView : UIPanelView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Image rarityBackgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text rarityText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text comparisonText;

        [Header("Action Buttons")]
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unequipButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button sellButton;

        [Header("Legacy Text Buttons")]
        [SerializeField] private Button equipOrUnequipButton;
        [SerializeField] private TMP_Text equipOrUnequipButtonText;
        [SerializeField] private Button buyOrSellButton;
        [SerializeField] private TMP_Text buyOrSellButtonText;
        [SerializeField] private GameObject unlockedActionsRoot;
        [SerializeField] private GameObject lockedActionsRoot;
        [SerializeField] private RarityBackgroundEntry[] rarityBackgrounds;

        private EquipmentItemDetailPanelViewModel _viewModel;
        private bool _isListening;

        public override PanelId PanelId => PanelId.ItemDetail;

        public void Bind(EquipmentItemDetailPanelViewModel viewModel)
        {
            _viewModel = viewModel;
            AddListeners();
            Refresh();
        }

        public void Unbind()
        {
            RemoveListeners();
            _viewModel = null;
        }

        public override void Show()
        {
            base.Show();
            Refresh();
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void AddListeners()
        {
            if (_isListening)
            {
                return;
            }

            closeButton?.onClick.AddListener(OnCloseClicked);
            equipButton?.onClick.AddListener(OnEquipClicked);
            unequipButton?.onClick.AddListener(OnUnequipClicked);
            buyButton?.onClick.AddListener(OnBuyClicked);
            sellButton?.onClick.AddListener(OnSellClicked);
            equipOrUnequipButton?.onClick.AddListener(OnEquipOrUnequipClicked);
            buyOrSellButton?.onClick.AddListener(OnBuyOrSellClicked);
            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            closeButton?.onClick.RemoveListener(OnCloseClicked);
            equipButton?.onClick.RemoveListener(OnEquipClicked);
            unequipButton?.onClick.RemoveListener(OnUnequipClicked);
            buyButton?.onClick.RemoveListener(OnBuyClicked);
            sellButton?.onClick.RemoveListener(OnSellClicked);
            equipOrUnequipButton?.onClick.RemoveListener(OnEquipOrUnequipClicked);
            buyOrSellButton?.onClick.RemoveListener(OnBuyOrSellClicked);
            _isListening = false;
        }

        private void Refresh()
        {
            if (_viewModel == null)
            {
                return;
            }

            EquipmentItemModel item = _viewModel.CurrentItem;
            if (item == null)
            {
                return;
            }

            if (iconImage != null)
            {
                iconImage.sprite = item.Icon;
                iconImage.enabled = item.Icon != null;
            }

            if (rarityBackgroundImage != null)
            {
                Sprite background = RarityBackgroundResolver.Resolve(rarityBackgrounds, item.Rarity);
                rarityBackgroundImage.sprite = background;
                rarityBackgroundImage.enabled = background != null;
            }

            if (nameText != null)
            {
                nameText.text = $"Name: {item.DisplayName}";
            }

            if (rarityText != null)
            {
                rarityText.text = $"Rarity: {item.Rarity}";
            }

            if (descriptionText != null)
            {
                descriptionText.text = $"Description: {item.Description}";
            }

            if (priceText != null)
            {
                priceText.text = item.IsUnlocked ? $"Sell: {item.SellPrice}" : $"Buy: {item.BuyPrice}";
            }

            if (statsText != null)
            {
                statsText.text = BuildStatsText(item);
            }

            if (comparisonText != null)
            {
                comparisonText.text = _viewModel.CanCompare ? BuildComparisonText(item, _viewModel.GetComparedItem()) : string.Empty;
            }

            if (unlockedActionsRoot != null)
            {
                unlockedActionsRoot.SetActive(item.IsUnlocked);
            }

            if (lockedActionsRoot != null)
            {
                lockedActionsRoot.SetActive(!item.IsUnlocked);
            }

            bool usesImageButtons = equipButton != null || unequipButton != null || buyButton != null || sellButton != null;
            bool isEquipped = _viewModel.IsCurrentItemEquipped;

            if (equipButton != null)
            {
                equipButton.gameObject.SetActive(item.IsUnlocked && !isEquipped);
            }

            if (unequipButton != null)
            {
                unequipButton.gameObject.SetActive(item.IsUnlocked && isEquipped);
            }

            if (buyButton != null)
            {
                buyButton.gameObject.SetActive(!item.IsUnlocked);
            }

            if (sellButton != null)
            {
                sellButton.gameObject.SetActive(item.IsUnlocked);
            }

            if (equipOrUnequipButton != null)
            {
                equipOrUnequipButton.gameObject.SetActive(!usesImageButtons);
                equipOrUnequipButton.interactable = item.IsUnlocked;
            }

            if (equipOrUnequipButtonText != null)
            {
                equipOrUnequipButtonText.text = isEquipped ? "Unequip" : "Equip";
            }

            if (buyOrSellButton != null)
            {
                buyOrSellButton.gameObject.SetActive(!usesImageButtons);
            }

            if (buyOrSellButtonText != null)
            {
                buyOrSellButtonText.text = item.IsUnlocked ? "Sell" : "Buy";
            }

        }

        private void OnCloseClicked()
        {
            _viewModel?.Close();
        }

        private void OnEquipOrUnequipClicked()
        {
            _viewModel?.RequestEquipOrUnequip();
            Refresh();
        }

        private void OnEquipClicked()
        {
            _viewModel?.RequestEquipOrUnequip();
            Refresh();
        }

        private void OnUnequipClicked()
        {
            _viewModel?.RequestEquipOrUnequip();
            Refresh();
        }

        private void OnSellClicked()
        {
            _viewModel?.RequestSell(Refresh);
        }

        private void OnBuyClicked()
        {
            _viewModel?.RequestBuy(Refresh);
        }

        private void OnBuyOrSellClicked()
        {
            EquipmentItemModel item = _viewModel?.CurrentItem;
            if (item == null)
            {
                return;
            }

            if (item.IsUnlocked)
            {
                _viewModel.RequestSell(Refresh);
                return;
            }

            _viewModel.RequestBuy(Refresh);
        }

        private static string BuildStatsText(EquipmentItemModel item)
        {
            return $"Item Stats: \n> ATK +{item.Damage}\n> DEF +{item.Armor}\n> HP +{item.MaxHealth}\n> SPD {FormatSigned(item.MoveSpeed)}";
        }

        private static string BuildComparisonText(EquipmentItemModel selected, EquipmentItemModel compared)
        {
            if (compared == null)
            {
                return string.Empty;
            }

            return $"Compare with {compared.DisplayName}\n"
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
