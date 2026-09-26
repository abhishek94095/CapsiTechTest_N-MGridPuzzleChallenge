using System;
using AV.Framework.Core.Board;
using AV.Framework.Core.Grid;
using MessagePipe;
using VContainer.Unity;

namespace AV.Framework.Application
{
    public sealed class PlayerMovement : IStartable, IDisposable
    {
        private readonly BoardInitializer boardInitializer;
        private readonly BoardPresenter boardPresenter;
        private readonly MoveHistory moveHistory;
        private readonly GameSession gameSession;
        private readonly GameFlow gameFlow;
        private readonly ISubscriber<UndoRequestedEvent> undoSubscriber;
        private IDisposable undoSubscription;
        private readonly IPublisher<GoalReachedEvent> goalPublisher;
        private readonly ISubscriber<GridDirection> subscriber;
        private IDisposable subscription;

        public PlayerMovement(
            BoardInitializer boardInitializer,
            BoardPresenter boardPresenter,
            MoveHistory moveHistory,
            GameSession gameSession,
            GameFlow gameFlow,
            ISubscriber<UndoRequestedEvent> undoSubscriber,
            IPublisher<GoalReachedEvent> goalPublisher,
            ISubscriber<GridDirection> subscriber)
        {
            this.boardInitializer = boardInitializer ?? throw new ArgumentNullException(nameof(boardInitializer));
            this.boardPresenter = boardPresenter ?? throw new ArgumentNullException(nameof(boardPresenter));
            this.moveHistory = moveHistory ?? throw new ArgumentNullException(nameof(moveHistory));
            this.gameSession = gameSession ?? throw new ArgumentNullException(nameof(gameSession));
            this.gameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            this.undoSubscriber = undoSubscriber ?? throw new ArgumentNullException(nameof(undoSubscriber));
            this.goalPublisher = goalPublisher ?? throw new ArgumentNullException(nameof(goalPublisher));
            this.subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        }

        public void Start()
        {
            subscription = subscriber.Subscribe(OnDirectionReceived);
            undoSubscription = undoSubscriber.Subscribe(OnUndoRequested);
        }

        private void OnDirectionReceived(GridDirection direction)
        {
            if (gameFlow.State != GameFlowState.Playing) return;

            UnityEngine.Debug.Log($"Movement received: {direction}");

            Board board = boardInitializer.CurrentBoard;
            if (board == null)
            {
                UnityEngine.Debug.LogWarning("Movement ignored: board is not initialized.");
                return;
            }

            if (!board.TryGetPiece(-1, out Piece player)) return;
            GridPosition previousPosition = player.Position;

            if (!gameSession.TryConsumeMove())
            {
                UnityEngine.Debug.Log("Movement blocked: move limit reached.");
                return;
            }

            if (!board.TryMovePlayer(direction, out int killedPieceId))
            {
                gameSession.RestoreMove();
                UnityEngine.Debug.Log($"Movement blocked: {direction}");
                return;
            }

            moveHistory.Add(new Move(previousPosition, direction, killedPieceId));

            if (board.TryGetPiece(-1, out player))
            {
                UnityEngine.Debug.Log($"Player moved to: {player.Position}, Killed piece: {killedPieceId}");
                boardPresenter.UpdatePiecePosition(player.Id, player.Position);

                if (board.Grid.TryGetCell(player.Position, out GridCell cell) && cell.CellType == CellType.Goal)
                {
                    UnityEngine.Debug.Log("Goal reached!");
                    goalPublisher.Publish(new GoalReachedEvent(player.Id));
                }
            }

            if (killedPieceId != -1)
            {
                boardPresenter.RemovePiece(killedPieceId);
            }
        }

        private void OnUndoRequested(UndoRequestedEvent _)
        {
            if (gameFlow.State != GameFlowState.Playing) return;

            Board board = boardInitializer.CurrentBoard;
            if (board == null) return;
            if (!moveHistory.TryRemoveLast(out Move move)) return;

            if (!board.TryUndoMove(move))
            {
                moveHistory.Add(move);
                UnityEngine.Debug.Log("Undo blocked: previous board position is unavailable.");
                return;
            }

            gameSession.RestoreMove();

            if (!board.TryGetPiece(-1, out Piece player)) return;

            boardPresenter.UpdatePiecePosition(player.Id, player.Position);

            if (move.KilledPieceId >= 0 && board.TryGetPiece(move.KilledPieceId, out Piece restoredPiece))
            {
                boardPresenter.RenderPiece(restoredPiece);
            }
        }

        public void Dispose()
        {
            subscription?.Dispose();
            undoSubscription?.Dispose();
        }
    }
}