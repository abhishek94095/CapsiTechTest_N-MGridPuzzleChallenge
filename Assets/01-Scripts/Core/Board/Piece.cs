using AV.Framework.Core.Grid;

namespace AV.Framework.Core.Board
{
    public readonly struct Piece
    {
        public int Id { get; }
        public PieceType Type { get; }
        public GridPosition Position { get; }
        public bool IsActive { get; }
        public GridPosition[] Path { get; }

        public Piece(
            int id,
            PieceType type,
            GridPosition position,
            bool isActive,
            GridPosition[] path)
        {
            Id = id;
            Type = type;
            Position = position;
            IsActive = isActive;
            Path = path;
        }

        public Piece WithPosition(GridPosition position) => new Piece(Id, Type, position, IsActive, Path);

        public Piece WithActiveState(bool isActive) => new Piece(Id, Type, Position, isActive, Path);
    }
}
