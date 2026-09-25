using System;
using System.Collections.Generic;
using AV.Framework.Core.Board;
using AV.Framework.Core.Grid;
using AV.Framework.GameData;
using UnityEngine;
using CoreBoard = AV.Framework.Core.Board.Board;

namespace AV.Framework.Application
{
    public sealed class BoardFactory
    {
        private const int PlayerPieceId = -1;

        public CoreBoard Create(BoardData boardData)
        {
            if (boardData == null) throw new ArgumentNullException(nameof(boardData));
            if (boardData.Cells == null) throw new ArgumentException("Board cells are not configured.", nameof(boardData));
            if (boardData.Cells.Length != boardData.Width * boardData.Height) throw new ArgumentException("Board cell count does not match board dimensions.", nameof(boardData));

            GridPosition startPosition = FindStartPosition(boardData.Cells, boardData.Width, boardData.Height);
            GridCell[] cells = CreateCells(boardData.Cells, boardData.Width, boardData.Height);
            List<Piece> pieces = new List<Piece>();
            HashSet<int> pieceIds = new HashSet<int> { PlayerPieceId };

            int configuredPieceCount = boardData.Pieces == null ? 0 : boardData.Pieces.Length;
            int maxPieceCount = Math.Min(boardData.Width, boardData.Height) - 1;
            if (configuredPieceCount + 1 > maxPieceCount) throw new ArgumentException("Board contains too many pieces for its dimensions.", nameof(boardData));

            pieces.Add(new Piece(PlayerPieceId, PieceType.Player, startPosition, true, Array.Empty<GridPosition>()));
            cells[startPosition.Y * boardData.Width + startPosition.X] = cells[startPosition.Y * boardData.Width + startPosition.X].WithOccupant(CellOccupant.Player);

            if (boardData.Pieces != null)
            {
                for (int index = 0; index < boardData.Pieces.Length; index++)
                {
                    BoardData.PieceData pieceData = boardData.Pieces[index];
                    GridPosition position = GridPosition.FromVector2Int(pieceData.StartPosition);

                    if (!pieceIds.Add(pieceData.Id)) throw new ArgumentException($"Piece ID {pieceData.Id} is duplicated.", nameof(boardData));
                    if (!IsValidPosition(position, boardData.Width, boardData.Height)) throw new ArgumentException($"Piece {pieceData.Id} is outside the board.", nameof(boardData));

                    int cellIndex = position.Y * boardData.Width + position.X;
                    if (cells[cellIndex].IsBlocked) throw new ArgumentException($"Piece {pieceData.Id} is placed on a stone cell.", nameof(boardData));
                    if (cells[cellIndex].IsOccupied) throw new ArgumentException($"Cell {position} already contains a piece.", nameof(boardData));

                    GridPosition[] path = ConvertPath(pieceData.Path);
                    pieces.Add(new Piece(pieceData.Id, pieceData.Type, position, true, path));
                    cells[cellIndex] = cells[cellIndex].WithOccupant(CellOccupant.Obstacle);
                }
            }

            Core.Grid.Grid grid = new Core.Grid.Grid(new GridSize(boardData.Width, boardData.Height), cells);
            return new CoreBoard(grid, pieces.ToArray());
        }

        private static GridPosition FindStartPosition(CellType[] cellTypes, int width, int height)
        {
            GridPosition startPosition = default;
            int startCount = 0;

            for (int index = 0; index < cellTypes.Length; index++)
            {
                if (cellTypes[index] != CellType.Start) continue;

                startPosition = new GridPosition(index % width, index / width);
                startCount++;
            }

            if (startCount != 1) throw new ArgumentException("Board must contain exactly one Start cell.");

            return startPosition;
        }

        private static GridCell[] CreateCells(CellType[] cellTypes, int width, int height)
        {
            GridCell[] cells = new GridCell[cellTypes.Length];

            for (int index = 0; index < cells.Length; index++)
            {
                GridPosition position = new GridPosition(index % width, index / width);
                cells[index] = new GridCell(position, cellTypes[index], CellOccupant.None);
            }

            return cells;
        }

        private static GridPosition[] ConvertPath(Vector2Int[] path)
        {
            if (path == null || path.Length == 0) return Array.Empty<GridPosition>();

            GridPosition[] convertedPath = new GridPosition[path.Length];

            for (int index = 0; index < path.Length; index++)
            {
                convertedPath[index] = GridPosition.FromVector2Int(path[index]);
            }

            return convertedPath;
        }

        private static bool IsValidPosition(GridPosition position, int width, int height)
        {
            return position.X >= 0 && position.X < width && position.Y >= 0 && position.Y < height;
        }
    }
}