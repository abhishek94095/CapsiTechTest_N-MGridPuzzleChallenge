namespace AV.Framework.Core.Grid
{
    public readonly struct GridCell
    {
        public GridPosition Position { get; }
        public CellType CellType { get; }
        public CellOccupant Occupant { get; }
        public bool IsBlocked => CellType == CellType.Stone;
        public bool IsOccupied => Occupant != CellOccupant.None;

        public GridCell(GridPosition position, CellType cellType, CellOccupant occupant)
        {
            Position = position;
            CellType = cellType;
            Occupant = occupant;
        }

        public GridCell WithCellType(CellType cellType) => new GridCell(Position, cellType, Occupant);
        public GridCell WithOccupant(CellOccupant occupant) => new GridCell(Position, CellType, occupant);
    }
}
