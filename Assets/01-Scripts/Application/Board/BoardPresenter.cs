using System;
using System.Collections.Generic;
using AV.Framework.Core.Board;
using AV.Framework.Core.Grid;
using AV.Framework.GameData;
using UnityEngine;

namespace AV.Framework.Application
{
    public sealed class BoardPresenter
    {
        private readonly BoardVisualConfig visualConfig;
        private readonly Dictionary<int, GameObject> pieceObjects = new Dictionary<int, GameObject>();
        private readonly Dictionary<GridPosition, GameObject> cellObjects = new Dictionary<GridPosition, GameObject>();
        private Transform boardRoot;
        private int boardWidth;
        private int boardHeight;

        public BoardPresenter(BoardVisualConfig visualConfig)
        {
            this.visualConfig = visualConfig ?? throw new ArgumentNullException(nameof(visualConfig));
        }

        public void Present(Board board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));

            Clear();
            boardWidth = board.Grid.Size.Width;
            boardHeight = board.Grid.Size.Height;
            boardRoot = new GameObject("Board").transform;

            PresentCells(board.Grid);
            PresentPieces(board);
        }

        public bool TryGetGridPosition(Vector2 screenPosition, Camera gameCamera, out GridPosition position)
        {
            if (gameCamera == null)
            {
                position = default;
                return false;
            }

            Vector3 worldPosition = gameCamera.ScreenToWorldPoint(new Vector3(
                screenPosition.x,
                screenPosition.y,
                -gameCamera.transform.position.z));
            int x = Mathf.FloorToInt(worldPosition.x + boardWidth * 0.5f);
            int y = Mathf.FloorToInt(worldPosition.y + boardHeight * 0.5f);

            position = new GridPosition(x, y);
            return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
        }

        private void PresentCells(Core.Grid.Grid grid)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    GridPosition position = new GridPosition(x, y);
                    if (!grid.TryGetCell(position, out GridCell cell)) continue;

                    GameObject prefab = GetCellPrefab(cell.CellType);
                    if (prefab == null) throw new InvalidOperationException($"No prefab configured for cell type {cell.CellType}.");

                    GameObject cellObject = Instantiate(prefab, position, $"Cell_{x}_{y}");
                    cellObjects[position] = cellObject;
                }
            }
        }

        private void PresentPieces(Board board)
        {
            for (int index = 0; index < board.PieceCount; index++)
            {
                Piece piece = board.Pieces[index];
                if (!piece.IsActive) continue;

                GameObject prefab = piece.Type == PieceType.Player ? visualConfig.PlayerPrefab : visualConfig.ObstaclePrefab;
                if (prefab == null) throw new InvalidOperationException($"No prefab configured for piece type {piece.Type}.");

                GameObject pieceObject = Instantiate(prefab, piece.Position, $"Piece_{piece.Id}");
                pieceObjects.Add(piece.Id, pieceObject);
            }
        }

        private GameObject GetCellPrefab(CellType cellType)
        {
            if (cellType == CellType.Stone) return visualConfig.StoneCellPrefab;
            if (cellType == CellType.Start) return visualConfig.StartCellPrefab;
            if (cellType == CellType.Goal) return visualConfig.GoalCellPrefab;
            return visualConfig.NormalCellPrefab;
        }

        private GameObject Instantiate(GameObject prefab, GridPosition position, string objectName)
        {
            GameObject instance = UnityEngine.Object.Instantiate(prefab, GetWorldPosition(position), Quaternion.identity, boardRoot);
            instance.name = objectName;
            return instance;
        }

        public void UpdatePiecePosition(int pieceId, GridPosition position)
        {
            if (!pieceObjects.TryGetValue(pieceId, out GameObject pieceObject)) return;
            pieceObject.transform.position = GetWorldPosition(position);
        }

        public void HidePiece(int pieceId)
        {
            if (!pieceObjects.TryGetValue(pieceId, out GameObject pieceObject)) return;

            pieceObject.SetActive(false);
        }

        public void HidePieceAtPosition(GridPosition position)
        {
            Vector3 targetPosition = GetWorldPosition(position);
            foreach (KeyValuePair<int, GameObject> entry in pieceObjects)
            {
                if (!entry.Value.activeSelf) continue;
                if (Vector3.Distance(entry.Value.transform.position, targetPosition) > 0.01f) continue;

                entry.Value.SetActive(false);
                return;
            }
        }

        public void RenderPiece(Piece piece)
        {
            if (pieceObjects.ContainsKey(piece.Id))
            {
                pieceObjects[piece.Id].SetActive(piece.IsActive);
                UpdatePiecePosition(piece.Id, piece.Position);
                return;
            }

            GameObject prefab = piece.Type == PieceType.Player ? visualConfig.PlayerPrefab : visualConfig.ObstaclePrefab;
            if (prefab == null) throw new InvalidOperationException($"No prefab configured for piece type {piece.Type}.");

            pieceObjects[piece.Id] = Instantiate(prefab, piece.Position, piece.Type.ToString());
        }

        public void UpdateCellVisual(GridPosition position, CellType cellType)
        {
            if (!cellObjects.TryGetValue(position, out GameObject existingCell)) return;

            GameObject prefab = GetCellPrefab(cellType);
            if (prefab == null) throw new InvalidOperationException($"No prefab configured for cell type {cellType}.");

            existingCell.SetActive(false);
            cellObjects[position] = Instantiate(prefab, position, $"Cell_{position.X}_{position.Y}_Visual");
        }

        private Vector3 GetWorldPosition(GridPosition position)
        {
            float x = position.X - (boardWidth - 1) * 0.5f;
            float y = position.Y - (boardHeight - 1) * 0.5f;
            return new Vector3(x, y, 0f);
        }

        public void Clear()
        {
            if (boardRoot != null) UnityEngine.Object.Destroy(boardRoot.gameObject);

            boardRoot = null;
            pieceObjects.Clear();
            cellObjects.Clear();
        }
    }
}