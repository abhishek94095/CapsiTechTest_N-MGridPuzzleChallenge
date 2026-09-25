using System;
using AV.Framework.Core.Board;
using AV.Framework.Core.Grid;
using UnityEngine;

namespace AV.Framework.GameData
{
    [CreateAssetMenu(fileName = "BoardData", menuName = "Game/Board Data")]
    public sealed class BoardData : ScriptableObject
    {
        [SerializeField] private int width = 5;
        [SerializeField] private int height = 8;
        [SerializeField] private CellType[] cells;
        [SerializeField] private PieceData[] pieces;

        public int Width => width;
        public int Height => height;
        public CellType[] Cells => cells;
        public PieceData[] Pieces => pieces;

        [Serializable]
        public sealed class PieceData
        {
            [SerializeField] private int id;
            [SerializeField] private PieceType type;
            [SerializeField] private Vector2Int startPosition;
            [SerializeField] private Vector2Int[] path;

            public int Id => id;
            public PieceType Type => type;
            public Vector2Int StartPosition => startPosition;
            public Vector2Int[] Path => path;
        }
    }
}