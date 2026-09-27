using System;
using System.Collections.Generic;
using AV.Framework.Core.Board;

namespace AV.Framework.Application
{
    public sealed class MoveHistory
    {
        private readonly Stack<Move> moves = new Stack<Move>();

        public int Count => moves.Count;
        public event Action<int> CountChanged;

        public void Add(Move move)
        {
            moves.Push(move);
            CountChanged?.Invoke(moves.Count);
        }

        public bool TryRemoveLast(out Move move)
        {
            if (moves.Count == 0)
            {
                move = default;
                return false;
            }

            move = moves.Pop();
            CountChanged?.Invoke(moves.Count);
            return true;
        }

        public void Clear()
        {
            if (moves.Count == 0) return;

            moves.Clear();
            CountChanged?.Invoke(0);
        }
    }
}