namespace AV.Framework.Core.Grid
{
    public sealed class Grid
    {
        private readonly GridSize size;
        private readonly GridCell[] cells;

        public GridSize Size => size;

        public Grid(int width, int height)
        {
            size = new GridSize(width, height);
            cells = new GridCell[size.CellCount];

            for (int index = 0; index < cells.Length; index++)
            {
                int x = index % size.Width;
                int y = index / size.Width;
                GridPosition position = new GridPosition(x, y);
                cells[index] = new GridCell(position, CellType.Normal, CellOccupant.None);
            }
        }

        public Grid(GridSize gridSize, GridCell[] gridCells)
        {
            if (gridCells == null) throw new System.ArgumentNullException(nameof(gridCells));
            if (gridCells.Length != gridSize.CellCount) throw new System.ArgumentException("Grid cell count does not match grid size.", nameof(gridCells));

            size = gridSize;
            cells = gridCells;
        }

        public bool IsValidPosition(GridPosition position)
        {
            return position.X >= 0 && position.X < size.Width && position.Y >= 0 && position.Y < size.Height;
        }

        public bool TryGetCell(GridPosition position, out GridCell cell)
        {
            if (!IsValidPosition(position))
            {
                cell = default;
                return false;
            }

            cell = cells[GetIndex(position)];
            return true;
        }

        internal void SetCell(GridCell cell)
        {
            cells[GetIndex(cell.Position)] = cell;
        }

        private int GetIndex(GridPosition position) => position.Y * size.Width + position.X;
    }
}
