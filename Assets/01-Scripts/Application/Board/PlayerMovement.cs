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
        private readonly IPublisher<GoalReachedEvent> goalPublisher;
        private readonly ISubscriber<GridDirection> subscriber;
        private IDisposable subscription;

        public PlayerMovement(
            BoardInitializer boardInitializer,
            BoardPresenter boardPresenter,
            IPublisher<GoalReachedEvent> goalPublisher,
            ISubscriber<GridDirection> subscriber)
        {
            this.boardInitializer = boardInitializer ?? throw new ArgumentNullException(nameof(boardInitializer));
            this.boardPresenter = boardPresenter ?? throw new ArgumentNullException(nameof(boardPresenter));
            this.goalPublisher = goalPublisher ?? throw new ArgumentNullException(nameof(goalPublisher));
            this.subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        }

        public void Start()
        {
            subscription = subscriber.Subscribe(OnDirectionReceived);
        }

        private void OnDirectionReceived(GridDirection direction)
        {
            UnityEngine.Debug.Log($"Movement received: {direction}");

            Board board = boardInitializer.CurrentBoard;
            if (board == null)
            {
                UnityEngine.Debug.LogWarning("Movement ignored: board is not initialized.");
                return;
            }

            if (!board.TryMovePlayer(direction, out int killedPieceId))
            {
                UnityEngine.Debug.Log($"Movement blocked: {direction}");
                return;
            }

            if (board.TryGetPiece(-1, out Piece player))
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

        public void Dispose()
        {
            subscription.Dispose();
        }
    }
}