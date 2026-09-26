using AV.Framework.Core.Grid;

namespace AV.Framework.Core.Board
{
    public readonly struct Move
    {
        public GridPosition Position { get; }
        public GridDirection Direction { get; }
        public int KilledPieceId { get; }

        public Move(GridPosition position, GridDirection direction, int killedPieceId)
        {
            Position = position;
            Direction = direction;
            KilledPieceId = killedPieceId;
        }
    }
}