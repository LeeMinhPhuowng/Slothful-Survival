using UnityEngine;

namespace Core.Foundation.Enums
{

    public enum EconomyError
    {
        None,
        InsufficientEnergy,
        InsufficientGold,
        InsufficientGems,
        AtMaxCap,           // Tried to add but already at HardCap
        InvalidAmount       // Negative or zero amount
    }
}
