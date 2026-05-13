using System.Collections.Generic;
using System;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.Service;
using R3;
using System.Diagnostics;

namespace Game.UI.Presentation.Profile
{
    public sealed class ProfilePanelViewModel
    {
        private readonly IPanelService _panelService;
        private readonly ICharacterRosterService _characterRosterService;
        private readonly IInventoryService _inventoryService;
        private readonly IWalletService _walletService;
        private readonly EquipmentItemDetailState _detailState;

        public ReadOnlyReactiveProperty<CharacterModel> SelectedCharacter => _characterRosterService.SelectedCharacter;
        public CharacterModel CurrentSelectedCharacter => _characterRosterService.GetSelectedCharacter();
        public EquipmentSlot? CurrentSelectedSlot { get; private set; }
        public EquipmentTab CurrentSelectedTab => _inventoryService.SelectedTab;
        public IReadOnlyList<EquipmentItemModel> CurrentFilteredItems => _inventoryService.FilteredItems;
        public int CurrentGold => _walletService.GetGold();
        public int CurrentDiamond => _walletService.GetDiamond();
        public event Action Changed
        {
            add
            {
                _inventoryService.Changed += value;
                _walletService.Changed += value;
            }
            remove
            {
                _inventoryService.Changed -= value;
                _walletService.Changed -= value;
            }
        }

        public ProfilePanelViewModel(
            IPanelService panelService,
            ICharacterRosterService characterRosterService,
            IInventoryService inventoryService,
            IWalletService walletService,
            EquipmentItemDetailState detailState)
        {
            _panelService = panelService;
            _characterRosterService = characterRosterService;
            _inventoryService = inventoryService;
            _walletService = walletService;
            _detailState = detailState;
        }

        public void PrepareOpen()
        {
            CurrentSelectedSlot = null;
            _detailState.Clear();
            _inventoryService.SelectTab(EquipmentTab.All);
        }

        public void SelectPreviousCharacter()
        {
            _characterRosterService.SelectPrevious();
        }

        public void SelectNextCharacter()
        {
            _characterRosterService.SelectNext();
        }

        public void SelectTab(EquipmentTab tab)
        {
            CloseDependentPanels();
            CurrentSelectedSlot = tab == EquipmentTab.All ? null : ToSlot(tab);
            _inventoryService.SelectTab(tab);
        }

        public void SelectSlot(EquipmentSlot slot)
        {
            CloseDependentPanels();
            CurrentSelectedSlot = slot;
            _inventoryService.SelectTab(ToTab(slot));
        }

        public void OpenItemDetail(string itemId)
        {
            EquipmentItemModel item = _inventoryService.GetItem(itemId);
            
            if (item == null)
            {
                return;
            }
            
            CurrentSelectedSlot = item.Slot;
            _detailState.Select(item.ItemId);
            _panelService.Open(PanelId.ItemDetail);
        }

        public void Close()
        {
            CloseDependentPanels();
            _panelService.Close(PanelId.Profile);
        }

        public bool IsEquipped(EquipmentItemModel item)
        {
            return item != null && _inventoryService.IsEquipped(item.ItemId);
        }

        public EquipmentItemModel GetEquippedItem(EquipmentSlot slot)
        {
            return _inventoryService.EquippedItems.TryGetValue(slot, out EquipmentItemModel item) ? item : null;
        }

        public PlayerStatSummary GetTotalStats()
        {
            CharacterModel character = _characterRosterService.GetSelectedCharacter();
            int maxHealth = character?.Config != null ? character.Config.maxHealth : 0;
            int moveSpeed = character?.Config != null ? character.Config.moveSpeed : 0;
            int armor = 0;
            int damage = 0;

            foreach (EquipmentItemModel item in _inventoryService.EquippedItems.Values)
            {
                maxHealth += item.MaxHealth;
                moveSpeed += item.MoveSpeed;
                armor += item.Armor;
                damage += item.Damage;
            }

            return new PlayerStatSummary(maxHealth, damage, armor, moveSpeed);
        }

        private static EquipmentSlot ToSlot(EquipmentTab tab)
        {
            return tab switch
            {
                EquipmentTab.Armor => EquipmentSlot.Armor,
                EquipmentTab.Helmet => EquipmentSlot.Helmet,
                EquipmentTab.Boots => EquipmentSlot.Boots,
                EquipmentTab.Gloves => EquipmentSlot.Gloves,
                EquipmentTab.Pants => EquipmentSlot.Pants,
                EquipmentTab.Weapon => EquipmentSlot.Weapon,
                _ => EquipmentSlot.Weapon
            };
        }

        private static EquipmentTab ToTab(EquipmentSlot slot)
        {
            return slot switch
            {
                EquipmentSlot.Armor => EquipmentTab.Armor,
                EquipmentSlot.Helmet => EquipmentTab.Helmet,
                EquipmentSlot.Boots => EquipmentTab.Boots,
                EquipmentSlot.Gloves => EquipmentTab.Gloves,
                EquipmentSlot.Pants => EquipmentTab.Pants,
                EquipmentSlot.Weapon => EquipmentTab.Weapon,
                _ => EquipmentTab.All
            };
        }

        private void CloseDependentPanels()
        {
            _panelService.Close(PanelId.Confirmation);
            _panelService.Close(PanelId.ItemDetail);
            _detailState.Clear();
        }
    }
}
