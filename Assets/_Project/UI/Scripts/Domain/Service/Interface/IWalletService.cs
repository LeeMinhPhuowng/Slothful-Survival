using System;
using Game.UI.Data;
using R3;

namespace Game.UI.Service
{
    public interface IWalletService
    {
        event Action Changed;
        ReadOnlyReactiveProperty<int> Gold { get; }
        ReadOnlyReactiveProperty<int> Diamond { get; }
        int GetGold();
        int GetDiamond();
        bool HasEnough(CurrencyType currencyType, int amount);
        bool TrySpend(CurrencyType currencyType, int amount);
        void Add(CurrencyType currencyType, int amount, string source);
    }
}
