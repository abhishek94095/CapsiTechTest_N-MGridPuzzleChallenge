namespace AV.Framework.Core.Events
{
    using AV.Framework.Core.Grid;

    public readonly struct PlayerMovedEvent
    {
        public PlayerMovedEvent(GridPosition position)
        {
            Position = position;
        }

        public GridPosition Position { get; }
    }
}
