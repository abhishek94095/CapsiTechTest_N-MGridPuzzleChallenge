using UnityEngine;

namespace AV.Framework.Core.Grid
{
    public readonly struct GridPosition
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2Int ToVector2Int() => new Vector2Int(X, Y);

        public static GridPosition FromVector2Int(Vector2Int position)
        {
            return new GridPosition(position.x, position.y);
        }

        public bool Equals(GridPosition other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is GridPosition other && Equals(other);
        public override int GetHashCode() => System.HashCode.Combine(X, Y);
        public static bool operator ==(GridPosition left, GridPosition right) => left.Equals(right);
        public static bool operator !=(GridPosition left, GridPosition right) => !left.Equals(right);
        public override string ToString() => $"({X}, {Y})";
    }
}
