using System;

namespace AV.Framework.Application
{
    public sealed class GameSession
    {
        private int remainingMoves;

        public int RemainingMoves => remainingMoves;
        public event Action<int> RemainingMovesChanged;

        public void Start(int moveLimit)
        {
            if (moveLimit < 1) throw new ArgumentOutOfRangeException(nameof(moveLimit));
            remainingMoves = moveLimit;
            RemainingMovesChanged?.Invoke(remainingMoves);
        }

        public bool TryConsumeMove()
        {
            if (remainingMoves <= 0) return false;

            remainingMoves--;
            RemainingMovesChanged?.Invoke(remainingMoves);
            return true;
        }

        public void RestoreMove()
        {
            remainingMoves++;
            RemainingMovesChanged?.Invoke(remainingMoves);
        }
    }
}