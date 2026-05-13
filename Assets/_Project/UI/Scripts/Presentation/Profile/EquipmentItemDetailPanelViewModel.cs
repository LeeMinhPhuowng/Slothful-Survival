using System;
using Game.UI.Data;
using Game.UI.Model;
using Game.UI.Service;
using UnityEngine;

namespace Game.UI.Presentation.Profile
{
    public sealed class EquipmentItemDetailPanelViewModel
    {
        private readonly IPanelService _panelService;
        private readonly IInventoryService _inventoryService;
        private readonly IWalletService _walletService;
        private readonly IConfirmationDialogService _confirmationDialogService;
        private readonly EquipmentItemDetailState _detailState;

        public EquipmentItemDetailPanelViewModel(
            IPanelService panelService,
            IInventoryService inventoryService,
            IWalletService walletService,
            IConfirmationDialogService confirmationDialogService,
            EquipmentItemDetailState detailState)
        {
            _panelService = panelService;
            _inventoryService = inventoryService;
            _walletService = walletService;
            _confirmationDialogService = confirmationDialogService;
            _detailState = detailState;
        }

        public EquipmentItemModel CurrentItem => _inventoryService.GetItem(_detailState.SelectedItemId);
        public bool IsCurrentItemEquipped => CurrentItem != null && _inventoryService.IsEquipped(CurrentItem.ItemId);
        public bool CanCompare => CurrentItem != null && CurrentItem.IsUnlocked && !IsCurrentItemEquipped && GetComparedItem() != null;

        public EquipmentItemModel GetComparedItem()
        {
            EquipmentItemModel item = CurrentItem;
            if (item == null)
            {
                return null;
            }

            return _inventoryService.EquippedItems.TryGetValue(item.Slot, out EquipmentItemModel compared) ? compared : null;
        }

        public void RequestEquipOrUnequip()
        {
            EquipmentItemModel item = CurrentItem;
            if (item == null || !item.IsUnlocked)
            {
                return;
            }

            if (_inventoryService.IsEquipped(item.ItemId))
            {
                _inventoryService.Unequip(item.Slot);
                return;
            }
            _inventoryService.Equip(item.ItemId);
        }

        public void RequestBuy(Action afterTransaction = null)
        {
            EquipmentItemModel item = CurrentItem;
            if (item == null || item.IsUnlocked)
            {
                return;
            }

            if (!_walletService.HasEnough(CurrencyType.Gold, item.BuyPrice))
            {
                _confirmationDialogService.Request(
                    "Not enough money",
                    $"You don't have enough money to buy {item.DisplayName.Trim()}.",
                    null);
                return;
            }

            _confirmationDialogService.Request(
                "Xac nhan mua",
                $"Do you confirm BUYING the equipment {item.DisplayName.Trim()} for {item.BuyPrice} gold?",
                () =>
                {
                    Buy(item);
                    afterTransaction?.Invoke();
                });
        }

        public void RequestSell(Action afterTransaction = null)
        {
            EquipmentItemModel item = CurrentItem;
            if (item == null || !item.IsUnlocked)
            {
                return;
            }

            _confirmationDialogService.Request(
                "Xac nhan ban",
                $"Do you confirm SELLING the equipment {item.DisplayName.Trim()} for {item.SellPrice} gold?",
                () =>
                {
                    Sell(item);
                    afterTransaction?.Invoke();
                });
        }

        public void Close()
        {
            _panelService.Close(PanelId.ItemDetail);
        }

        private void Buy(EquipmentItemModel item)
        {
            if (!_walletService.TrySpend(CurrencyType.Gold, item.BuyPrice))
            {
                _confirmationDialogService.Request(
                    "Not enough money",
                    $"You don't have enough money to buy {item.DisplayName.Trim()} ({_walletService.Gold.CurrentValue/item.BuyPrice}).",
                    null);
                return;
            }

            _inventoryService.Unlock(item.ItemId);
        }

        private void Sell(EquipmentItemModel item)
        {
            _inventoryService.Lock(item.ItemId);
            _walletService.Add(CurrencyType.Gold, item.SellPrice, "equipment_sell");
        }
    }
}
