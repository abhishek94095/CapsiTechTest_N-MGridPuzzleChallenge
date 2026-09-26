using System.Collections.Generic;
using AV.Framework.Core.Board;

namespace AV.Framework.Application
{
    public sealed class MoveHistory
    {
        private readonly Stack<Move> moves = new Stack<Move>();

        public int Count => moves.Count;

        public void Add(Move move)
        {
            moves.Push(move);
        }

        public bool TryRemoveLast(out Move move)
        {
            if (moves.Count == 0)
            {
                move = default;
                return false;
            }

            move = moves.Pop();
            return true;
        }

        public void Clear()
        {
            moves.Clear();
        }
    }
}