using System;
using Game.UI.Data;
using R3;

namespace Game.UI.Service
{
    public sealed class WalletService : IWalletService
    {
        private readonly IPlayerProgressService _progressService;
        private readonly ReactiveProperty<int> _gold;
        private readonly ReactiveProperty<int> _diamond;

        public event Action Changed;

        public ReadOnlyReactiveProperty<int> Gold => _gold;
        public ReadOnlyReactiveProperty<int> Diamond => _diamond;

        public WalletService(IPlayerProgressService progressService)
        {
            _progressService = progressService;
            _gold = new ReactiveProperty<int>(_progressService.GetGold());
            _diamond = new ReactiveProperty<int>(_progressService.GetDiamond());
        }

        public bool HasEnough(CurrencyType currencyType, int amount)
        {
            return GetAmount(currencyType) >= amount;
        }

        public int GetGold()
        {
            return _gold.Value;
        }

        public int GetDiamond()
        {
            return _diamond.Value;
        }

        public bool TrySpend(CurrencyType currencyType, int amount)
        {
            if (amount < 0 || !HasEnough(currencyType, amount))
            {
                return false;
            }

            Add(currencyType, -amount, "spend");
            return true;
        }

        public void Add(CurrencyType currencyType, int amount, string source)
        {
            if (currencyType == CurrencyType.Gold)
            {
                int next = Math.Max(0, _gold.Value + amount);
                _gold.Value = next;
                _progressService.SetCurrency(CurrencyType.Gold, next);
                Changed?.Invoke();
                return;
            }

            int nextDiamond = Math.Max(0, _diamond.Value + amount);
            _diamond.Value = nextDiamond;
            _progressService.SetCurrency(CurrencyType.Diamond, nextDiamond);
            Changed?.Invoke();
        }

        private int GetAmount(CurrencyType currencyType)
        {
            return currencyType == CurrencyType.Gold ? _gold.Value : _diamond.Value;
        }
    }
}
