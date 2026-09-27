using System;
using System.Collections.Generic;
using AV.Framework.Core.Board;
using AV.Framework.Core.Events;
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
        private readonly GameFlow gameFlow;
        private readonly ISubscriber<PowerUpModeStartedEvent> powerUpStartedSubscriber;
        private readonly ISubscriber<PowerUpModeEndedEvent> powerUpEndedSubscriber;
        private readonly IPublisher<PlayerDeathEvent> deathPublisher;
        private readonly List<UniTask> movementTasks = new List<UniTask>();
        private IDisposable powerUpStartedSubscription;
        private IDisposable powerUpEndedSubscription;
        private bool isRunning;
        private bool isPowerUpModeActive;
        private int levelGeneration;

        public MovingPieceController(
            BoardInitializer boardInitializer,
            BoardPresenter boardPresenter,
            GameFlow gameFlow,
            ISubscriber<PowerUpModeStartedEvent> powerUpStartedSubscriber,
            ISubscriber<PowerUpModeEndedEvent> powerUpEndedSubscriber,
            IPublisher<PlayerDeathEvent> deathPublisher)
        {
            this.boardInitializer = boardInitializer ?? throw new ArgumentNullException(nameof(boardInitializer));
            this.boardPresenter = boardPresenter ?? throw new ArgumentNullException(nameof(boardPresenter));
            this.gameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            this.powerUpStartedSubscriber = powerUpStartedSubscriber ?? throw new ArgumentNullException(nameof(powerUpStartedSubscriber));
            this.powerUpEndedSubscriber = powerUpEndedSubscriber ?? throw new ArgumentNullException(nameof(powerUpEndedSubscriber));
            this.deathPublisher = deathPublisher ?? throw new ArgumentNullException(nameof(deathPublisher));
        }

        public void Start()
        {
            powerUpStartedSubscription = powerUpStartedSubscriber.Subscribe(OnPowerUpModeStarted);
            powerUpEndedSubscription = powerUpEndedSubscriber.Subscribe(OnPowerUpModeEnded);
        }

        public void StartLevel()
        {
            levelGeneration++;
            int currentGeneration = levelGeneration;
            isRunning = true;
            movementTasks.Clear();

            Board board = boardInitializer.CurrentBoard;
            if (board == null) return;

            for (int index = 0; index < board.PieceCount; index++)
            {
                Piece piece = board.Pieces[index];
                if (!piece.IsActive || piece.Type != PieceType.Obstacle || piece.Path == null || piece.Path.Length < 2) continue;

                movementTasks.Add(MovePieceAsync(piece.Id, currentGeneration));
            }
        }

        public void StopLevel()
        {
            levelGeneration++;
            isRunning = false;
            movementTasks.Clear();
        }

        public void Dispose()
        {
            StopLevel();
            powerUpStartedSubscription?.Dispose();
            powerUpEndedSubscription?.Dispose();
        }

        private async UniTask MovePieceAsync(int pieceId, int currentGeneration)
        {
            Board board = boardInitializer.CurrentBoard;
            if (board == null) return;

            if (!board.TryGetPiece(pieceId, out Piece piece)) return;
            int pathIndex = GetPathIndex(piece);

            while (isRunning)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(MoveInterval));
                if (!isRunning || currentGeneration != levelGeneration) return;
                if (gameFlow.State != GameFlowState.Playing) break;
                if (isPowerUpModeActive) continue;

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

        private void OnPowerUpModeStarted(PowerUpModeStartedEvent powerUpModeStartedEvent)
        {
            isPowerUpModeActive = true;
        }

        private void OnPowerUpModeEnded(PowerUpModeEndedEvent powerUpModeEndedEvent)
        {
            isPowerUpModeActive = false;
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