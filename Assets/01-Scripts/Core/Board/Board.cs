using System;
using AV.Framework.Core.Grid;

namespace AV.Framework.Core.Board
{
    public sealed class Board
    {
        public Grid.Grid Grid { get; }
        private readonly Piece[] pieces;
        public int PieceCount => pieces.Length;

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
    }
}