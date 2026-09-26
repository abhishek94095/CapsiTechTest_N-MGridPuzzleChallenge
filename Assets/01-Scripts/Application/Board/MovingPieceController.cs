using System;
using System.Collections.Generic;
using AV.Framework.Core.Board;
using AV.Framework.Core.Grid;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace AV.Framework.Application
{
    public sealed class MovingPieceController : IStartable, IDisposable
    {
        private const int PlayerPieceId = -1;
        private const float MoveInterval = 0.5f;

        private readonly BoardInitializer boardInitializer;
        private readonly BoardPresenter boardPresenter;
        private readonly IPublisher<PlayerDeathEvent> deathPublisher;
        private readonly List<UniTask> movementTasks = new List<UniTask>();
        private bool isRunning;

        public MovingPieceController(
            BoardInitializer boardInitializer,
            BoardPresenter boardPresenter,
            IPublisher<PlayerDeathEvent> deathPublisher)
        {
            this.boardInitializer = boardInitializer ?? throw new ArgumentNullException(nameof(boardInitializer));
            this.boardPresenter = boardPresenter ?? throw new ArgumentNullException(nameof(boardPresenter));
            this.deathPublisher = deathPublisher ?? throw new ArgumentNullException(nameof(deathPublisher));
        }

        public void Start()
        {
            isRunning = true;
            Board board = boardInitializer.CurrentBoard;
            if (board == null) return;

            for (int index = 0; index < board.PieceCount; index++)
            {
                Piece piece = board.Pieces[index];
                if (!piece.IsActive || piece.Type != PieceType.Obstacle || piece.Path == null || piece.Path.Length < 2) continue;

                movementTasks.Add(MovePieceAsync(piece.Id));
            }
        }

        public void Dispose()
        {
            isRunning = false;
            movementTasks.Clear();
        }

        private async UniTask MovePieceAsync(int pieceId)
        {
            Board board = boardInitializer.CurrentBoard;
            if (board == null) return;

            if (!board.TryGetPiece(pieceId, out Piece piece)) return;
            int pathIndex = GetPathIndex(piece);

            while (isRunning)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(MoveInterval));
                if (!isRunning) return;

                if (!board.TryGetPiece(pieceId, out piece)) return;
                if (!piece.IsActive) continue;

                pathIndex = (pathIndex + 1) % piece.Path.Length;
                GridPosition targetPosition = piece.Path[pathIndex];

                if (board.TryGetPiece(PlayerPieceId, out Piece player) && player.IsActive && player.Position == targetPosition)
                {
                    Debug.Log($"Player killed by moving piece: {pieceId}");
                    deathPublisher.Publish(new PlayerDeathEvent(pieceId));
                    continue;
                }

                if (!board.TryMovePiece(pieceId, targetPosition)) continue;

                boardPresenter.UpdatePiecePosition(pieceId, targetPosition);
            }
        }

        private static int GetPathIndex(Piece piece)
        {
            for (int index = 0; index < piece.Path.Length; index++)
            {
                if (piece.Path[index] == piece.Position) return index;
            }

            return 0;
        }
    }
}