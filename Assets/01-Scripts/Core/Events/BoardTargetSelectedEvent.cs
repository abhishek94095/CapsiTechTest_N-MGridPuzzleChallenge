using AV.Framework.Core.Gameplay;
using AV.Framework.Core.Grid;

namespace AV.Framework.Core.Events
{
    public readonly struct BoardTargetSelectedEvent
    {
        public PowerUpType PowerUpType { get; }
        public GridPosition Position { get; }
        public bool IsValid { get; }

        public BoardTargetSelectedEvent(PowerUpType powerUpType, GridPosition position, bool isValid)
        {
            PowerUpType = powerUpType;
            Position = position;
            IsValid = isValid;
        }
    }
}
