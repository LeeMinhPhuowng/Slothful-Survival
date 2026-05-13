using System.Collections.Generic;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Presentation.Profile
{
    public sealed class ProfilePanelView : UIPanelView
    {
        [Header("Navigation")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button previousCharacterButton;
        [SerializeField] private Button nextCharacterButton;

        [Header("Player")]
        [SerializeField] private CharacterPreviewView characterPreviewView;
        [SerializeField] private EquipmentSlotView[] slotViews;
        [SerializeField] private TMP_Text totalStatsText;

        [Header("Wallet")]
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text diamondText;

        [Header("Inventory")]
        [SerializeField] private EquipmentTabButtonView[] tabViews;
        [SerializeField] private Transform itemGridRoot;
        [SerializeField] private InventoryItemView itemViewPrefab;

        private readonly List<InventoryItemView> _spawnedItemViews = new();
        private ProfilePanelViewModel _viewModel;
        private bool _isListening;

        public override PanelId PanelId => PanelId.Profile;

        public void Bind(ProfilePanelViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.Changed += RefreshAll;
            AddListeners();
            BindChildren();
            _viewModel.PrepareOpen();
            RefreshAll();
        }

        public void Unbind()
        {
            RemoveListeners();
            UnbindChildren();
            ClearItemGrid();
            if (_viewModel != null)
            {
                _viewModel.Changed -= RefreshAll;
            }

            _viewModel = null;
        }

        public override void Show()
        {
            base.Show();
            _viewModel?.PrepareOpen();
            RefreshAll();
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

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }

            if (previousCharacterButton != null)
            {
                previousCharacterButton.onClick.AddListener(OnPreviousCharacterClicked);
            }

            if (nextCharacterButton != null)
            {
                nextCharacterButton.onClick.AddListener(OnNextCharacterClicked);
            }

            _isListening = true;
        }

        private void RemoveListeners()
        {
            if (!_isListening)
            {
                return;
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(OnCloseClicked);
            }

            if (previousCharacterButton != null)
            {
                previousCharacterButton.onClick.RemoveListener(OnPreviousCharacterClicked);
            }

            if (nextCharacterButton != null)
            {
                nextCharacterButton.onClick.RemoveListener(OnNextCharacterClicked);
            }

            _isListening = false;
        }

        private void BindChildren()
        {
            if (slotViews != null)
            {
                foreach (EquipmentSlotView slotView in slotViews)
                {
                    if (slotView != null)
                    {
                        slotView.Bind(OnSlotClicked);
                    }
                }
            }

            if (tabViews != null)
            {
                foreach (EquipmentTabButtonView tabView in tabViews)
                {
                    if (tabView != null)
                    {
                        tabView.Bind(OnTabClicked);
                    }
                }
            }

        }

        private void UnbindChildren()
        {
            if (slotViews != null)
            {
                foreach (EquipmentSlotView slotView in slotViews)
                {
                    slotView?.Unbind();
                }
            }

            if (tabViews != null)
            {
                foreach (EquipmentTabButtonView tabView in tabViews)
                {
                    tabView?.Unbind();
                }
            }

        }

        private void RefreshAll()
        {
            if (_viewModel == null)
            {
                return;
            }

            RenderCharacter();
            RenderSlots();
            RenderTabs();
            RenderItemGrid();
            RenderTotalStats();
            RenderWallet();
        }

        private void RenderCharacter()
        {
            characterPreviewView?.Render(_viewModel.CurrentSelectedCharacter);
        }

        private void RenderSlots()
        {
            EquipmentSlot? selectedSlot = _viewModel.CurrentSelectedSlot;

            if (slotViews == null)
            {
                return;
            }

            foreach (EquipmentSlotView slotView in slotViews)
            {
                if (slotView == null)
                {
                    continue;
                }

                EquipmentItemModel equippedItem = _viewModel.GetEquippedItem(slotView.Slot);
                bool isSelected = selectedSlot.HasValue && selectedSlot.Value == slotView.Slot;
                slotView.Render(equippedItem, isSelected, false);
            }
        }

        private void RenderTabs()
        {
            if (tabViews == null)
            {
                return;
            }

            EquipmentTab selectedTab = _viewModel.CurrentSelectedTab;
            foreach (EquipmentTabButtonView tabView in tabViews)
            {
                tabView?.Render(tabView.Tab == selectedTab);
            }
        }

        private void RenderItemGrid()
        {
            ClearItemGrid();

            if (itemGridRoot == null || itemViewPrefab == null)
            {
                return;
            }

            foreach (EquipmentItemModel item in _viewModel.CurrentFilteredItems)
            {
                InventoryItemView itemView = Instantiate(itemViewPrefab, itemGridRoot);
                itemView.Bind(OnInventoryItemClicked);
                itemView.Render(item, _viewModel.IsEquipped(item));
                _spawnedItemViews.Add(itemView);
            }
        }

        private void RenderTotalStats()
        {
            if (totalStatsText == null)
            {
                return;
            }

            PlayerStatSummary stats = _viewModel.GetTotalStats();
            totalStatsText.text = $"HP: {stats.MaxHealth}\nATK: {stats.Damage}\nDEF: {stats.Armor}\nSPD: {stats.MoveSpeed}";
        }

        private void RenderWallet()
        {
            if (goldText != null)
            {
                goldText.text = _viewModel.CurrentGold.ToString();
            }

            if (diamondText != null)
            {
                diamondText.text = _viewModel.CurrentDiamond.ToString();
            }
        }

        private void ClearItemGrid()
        {
            foreach (InventoryItemView itemView in _spawnedItemViews)
            {
                if (itemView == null)
                {
                    continue;
                }

                itemView.Unbind();
                Destroy(itemView.gameObject);
            }

            _spawnedItemViews.Clear();
        }

        private void OnCloseClicked()
        {
            _viewModel?.Close();
        }

        private void OnPreviousCharacterClicked()
        {
            _viewModel?.SelectPreviousCharacter();
            RefreshAll();
        }

        private void OnNextCharacterClicked()
        {
            _viewModel?.SelectNextCharacter();
            RefreshAll();
        }

        private void OnSlotClicked(EquipmentSlot slot)
        {
            _viewModel?.SelectSlot(slot);
            RefreshAll();
        }

        private void OnTabClicked(EquipmentTab tab)
        {
            _viewModel?.SelectTab(tab);
            RefreshAll();
        }

        private void OnInventoryItemClicked(string itemId)
        {
            _viewModel?.OpenItemDetail(itemId);
            RefreshAll();
        }
    }
}
