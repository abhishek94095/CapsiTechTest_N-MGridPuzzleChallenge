namespace AV.Framework.Core.Events
{
    using AV.Framework.Core.Gameplay;
    using AV.Framework.Core.Grid;

    public readonly struct PowerUpUsedEvent
    {
        public PowerUpUsedEvent(PowerUpType powerUpType, GridPosition position)
        {
            PowerUpType = powerUpType;
            Position = position;
        }

        public PowerUpType PowerUpType { get; }
        public GridPosition Position { get; }
    }
}
