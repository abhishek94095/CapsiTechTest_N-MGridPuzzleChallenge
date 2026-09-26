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

        public bool TryRestoreOccupant(GridPosition position, CellOccupant occupant)
        {
            if (!IsValidPosition(position)) return false;

            int index = GetIndex(position);
            GridCell cell = cells[index];
            if (cell.IsBlocked || cell.IsOccupied) return false;

            cells[index] = cell.WithOccupant(occupant);
            return true;
        }

        public bool TryMoveOccupant(GridPosition from, GridPosition to, CellOccupant occupant)
        {
            if (!IsValidPosition(from) || !IsValidPosition(to)) return false;
            if (from == to) return false;

            int fromIndex = GetIndex(from);
            int toIndex = GetIndex(to);

            if (cells[fromIndex].Occupant != occupant) return false;
            if (cells[toIndex].IsBlocked || cells[toIndex].IsOccupied) return false;

            cells[fromIndex] = cells[fromIndex].WithOccupant(CellOccupant.None);
            cells[toIndex] = cells[toIndex].WithOccupant(occupant);
            return true;
        }

        public bool TryMoveOccupant(GridPosition from, GridPosition to, CellOccupant occupant, CellOccupant replaceableOccupant)
        {
            if (!IsValidPosition(from) || !IsValidPosition(to)) return false;
            if (from == to) return false;

            int fromIndex = GetIndex(from);
            int toIndex = GetIndex(to);

            if (cells[fromIndex].Occupant != occupant) return false;
            if (cells[toIndex].IsBlocked) return false;
            if (cells[toIndex].IsOccupied && cells[toIndex].Occupant != replaceableOccupant) return false;

            cells[fromIndex] = cells[fromIndex].WithOccupant(CellOccupant.None);
            cells[toIndex] = cells[toIndex].WithOccupant(occupant);
            return true;
        }

        internal void SetCell(GridCell cell)
        {
            cells[GetIndex(cell.Position)] = cell;
        }

        private int GetIndex(GridPosition position) => position.Y * size.Width + position.X;
    }
}
