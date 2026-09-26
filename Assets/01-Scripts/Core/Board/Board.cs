using System;
using System.Collections.Generic;
using AV.Framework.Core.Grid;

namespace AV.Framework.Core.Board
{
    public sealed class Board
    {
        public Grid.Grid Grid { get; }
        private readonly Piece[] pieces;
        public int PieceCount => pieces.Length;
        public IReadOnlyList<Piece> Pieces => pieces;

        public Board(Grid.Grid grid, Piece[] pieces)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));
            if (pieces == null) throw new ArgumentNullException(nameof(pieces));

            Grid = grid;
            this.pieces = pieces;
        }

        public bool TryGetPiece(int id, out Piece piece)
        {
            for (int index = 0; index < pieces.Length; index++)
            {
                if (pieces[index].Id != id) continue;

                piece = pieces[index];
                return true;
            }

            piece = default;
            return false;
        }

        public bool TryMovePlayer(GridDirection direction, out int killedPieceId)
        {
            killedPieceId = -1;

            if (!TryGetPiece(-1, out Piece player)) return false;

            GridPosition targetPosition = GetTargetPosition(player.Position, direction);
            if (!Grid.IsValidPosition(targetPosition)) return false;
            if (!Grid.TryGetCell(targetPosition, out GridCell targetCell)) return false;
            if (targetCell.IsBlocked) return false;

            if (targetCell.Occupant == CellOccupant.Obstacle)
            {
                if (!TryGetPieceAtPosition(targetPosition, out Piece obstacle)) return false;

                killedPieceId = obstacle.Id;
                pieces[GetPieceIndex(obstacle.Id)] = obstacle.WithActiveState(false);
                if (!Grid.TryMoveOccupant(player.Position, targetPosition, CellOccupant.Player, CellOccupant.Obstacle)) return false;

                pieces[GetPieceIndex(-1)] = player.WithPosition(targetPosition);
                return true;
            }

            if (targetCell.IsOccupied) return false;
            if (!Grid.TryMoveOccupant(player.Position, targetPosition, CellOccupant.Player)) return false;

            pieces[GetPieceIndex(-1)] = player.WithPosition(targetPosition);
            return true;
        }

        public bool TryMovePiece(int pieceId, GridPosition targetPosition)
        {
            if (!TryGetPiece(pieceId, out Piece piece)) return false;
            if (!piece.IsActive) return false;
            if (!Grid.IsValidPosition(targetPosition)) return false;
            if (!Grid.TryGetCell(targetPosition, out GridCell targetCell)) return false;
            if (targetCell.IsBlocked || targetCell.IsOccupied) return false;
            if (!Grid.TryMoveOccupant(piece.Position, targetPosition, CellOccupant.Obstacle)) return false;

            pieces[GetPieceIndex(pieceId)] = piece.WithPosition(targetPosition);
            return true;
        }

        public bool TryUndoMove(Move move)
        {
            if (!TryGetPiece(-1, out Piece player) || !player.IsActive) return false;
            if (!Grid.TryMoveOccupant(player.Position, move.Position, CellOccupant.Player)) return false;

            if (move.KilledPieceId >= 0)
            {
                if (!TryGetPiece(move.KilledPieceId, out Piece killedPiece)) return false;
                if (killedPiece.IsActive) return false;
                if (!Grid.TryRestoreOccupant(player.Position, CellOccupant.Obstacle)) return false;

                pieces[GetPieceIndex(move.KilledPieceId)] = killedPiece
                    .WithPosition(player.Position)
                    .WithActiveState(true);
            }

            pieces[GetPieceIndex(-1)] = player.WithPosition(move.Position);
            return true;
        }

        private static GridPosition GetTargetPosition(GridPosition position, GridDirection direction)
        {
            if (direction == GridDirection.Up) return new GridPosition(position.X, position.Y + 1);
            if (direction == GridDirection.Down) return new GridPosition(position.X, position.Y - 1);
            if (direction == GridDirection.Left) return new GridPosition(position.X - 1, position.Y);
            return new GridPosition(position.X + 1, position.Y);
        }

        private int GetPieceIndex(int id)
        {
            for (int index = 0; index < pieces.Length; index++)
            {
                if (pieces[index].Id == id) return index;
            }

            return -1;
        }

        private bool TryGetPieceAtPosition(GridPosition position, out Piece piece)
        {
            for (int index = 0; index < pieces.Length; index++)
            {
                Piece candidate = pieces[index];
                if (candidate.IsActive && candidate.Position == position)
                {
                    piece = candidate;
                    return true;
                }
            }

            piece = default;
            return false;
        }

    }
}