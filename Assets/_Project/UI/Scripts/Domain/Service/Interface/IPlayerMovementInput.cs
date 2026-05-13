using System.Numerics;

namespace Game.UI.Service
{
    public interface IPlayerMovementInput
    {
        Vector2 MoveVector { get; }
    }
}