using System;
using AV.Framework.Core.Board;
using AV.Framework.Core.Events;
using AV.Framework.Core.Gameplay;
using AV.Framework.Core.Grid;
using MessagePipe;
using VContainer.Unity;

namespace AV.Framework.Application
{
    public sealed class PowerUpController : IStartable, IDisposable
    {
        private readonly BoardInitializer boardInitializer;
        private readonly BoardPresenter boardPresenter;
        private readonly GameFlow gameFlow;
        private readonly ISubscriber<BoardTargetSelectedEvent> targetSubscriber;
        private readonly IPublisher<PowerUpModeStartedEvent> modeStartedPublisher;
        private readonly IPublisher<PowerUpModeEndedEvent> modeEndedPublisher;
        private readonly IPublisher<PowerUpUsedEvent> powerUpUsedPublisher;
        private readonly IPublisher<PowerUpCancelledEvent> powerUpCancelledPublisher;
        private IDisposable targetSubscription;

        public PowerUpController(
            BoardInitializer boardInitializer,
            BoardPresenter boardPresenter,
            GameFlow gameFlow,
            ISubscriber<BoardTargetSelectedEvent> targetSubscriber,
            IPublisher<PowerUpModeStartedEvent> modeStartedPublisher,
            IPublisher<PowerUpModeEndedEvent> modeEndedPublisher,
            IPublisher<PowerUpUsedEvent> powerUpUsedPublisher,
            IPublisher<PowerUpCancelledEvent> powerUpCancelledPublisher)
        {
            this.boardInitializer = boardInitializer ?? throw new ArgumentNullException(nameof(boardInitializer));
            this.boardPresenter = boardPresenter ?? throw new ArgumentNullException(nameof(boardPresenter));
            this.gameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            this.targetSubscriber = targetSubscriber ?? throw new ArgumentNullException(nameof(targetSubscriber));
            this.modeStartedPublisher = modeStartedPublisher ?? throw new ArgumentNullException(nameof(modeStartedPublisher));
            this.modeEndedPublisher = modeEndedPublisher ?? throw new ArgumentNullException(nameof(modeEndedPublisher));
            this.powerUpUsedPublisher = powerUpUsedPublisher ?? throw new ArgumentNullException(nameof(powerUpUsedPublisher));
            this.powerUpCancelledPublisher = powerUpCancelledPublisher ?? throw new ArgumentNullException(nameof(powerUpCancelledPublisher));
        }

        public int HammerCharges { get; private set; } = 3;
        public int RocketCharges { get; private set; } = 3;
        public PowerUpType? ActivePowerUp { get; private set; }
        public bool IsPowerUpActive => ActivePowerUp.HasValue;

        public void Start()
        {
            targetSubscription = targetSubscriber.Subscribe(OnTargetSelected);
            gameFlow.StateChanged += OnGameFlowStateChanged;
        }

        public bool TryActivatePowerUp(PowerUpType powerUpType)
        {
            if (gameFlow.State != GameFlowState.Playing) return false;
            if (IsPowerUpActive) return false;

            int charges = GetCharges(powerUpType);
            if (charges <= 0) return false;

            ActivePowerUp = powerUpType;
            modeStartedPublisher.Publish(new PowerUpModeStartedEvent(powerUpType));
            return true;
        }

        public void ActivateHammer() => TryActivatePowerUp(PowerUpType.Hammer);

        public void ActivateRocket() => TryActivatePowerUp(PowerUpType.Rocket);

        public void CancelPowerUp()
        {
            EndPowerUpMode(true);
        }

        private void EndPowerUpMode(bool wasCancelled)
        {
            if (!IsPowerUpActive) return;

            PowerUpType powerUpType = ActivePowerUp.Value;
            ActivePowerUp = null;
            if (wasCancelled)
            {
                powerUpCancelledPublisher.Publish(new PowerUpCancelledEvent(powerUpType));
            }

            modeEndedPublisher.Publish(new PowerUpModeEndedEvent(powerUpType));
        }

        public void Dispose()
        {
            targetSubscription?.Dispose();
            gameFlow.StateChanged -= OnGameFlowStateChanged;
        }

        private void OnTargetSelected(BoardTargetSelectedEvent target)
        {
            if (!IsPowerUpActive) return;
            if (target.PowerUpType != ActivePowerUp.Value) return;
            if (gameFlow.State != GameFlowState.Playing)
            {
                CancelPowerUp();
                return;
            }

            if (!target.IsValid)
            {
                CancelPowerUp();
                return;
            }

            Board board = boardInitializer.CurrentBoard;
            if (board == null)
            {
                CancelPowerUp();
                return;
            }

            PowerUpType powerUpType = ActivePowerUp.Value;
            if (!TryApplyPowerUp(board, target.Position, powerUpType))
            {
                CancelPowerUp();
                return;
            }

            powerUpUsedPublisher.Publish(new PowerUpUsedEvent(powerUpType, target.Position));
            ConsumeCharge(powerUpType);
            EndPowerUpMode(false);
        }

        private bool TryApplyPowerUp(Board board, GridPosition position, PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.Hammer)
            {
                if (!board.TryHammerPiece(position, out int hammeredPieceId)) return false;

                boardPresenter.HidePiece(hammeredPieceId);
                return true;
            }

            if (!board.TryRocketWall(position)) return false;

            boardPresenter.UpdateCellVisual(position, CellType.Normal);
            return true;
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            if (state == GameFlowState.Won || state == GameFlowState.Lost)
            {
                EndPowerUpMode(false);
            }
        }

        private int GetCharges(PowerUpType powerUpType)
        {
            return powerUpType == PowerUpType.Hammer ? HammerCharges : RocketCharges;
        }

        private void ConsumeCharge(PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.Hammer)
            {
                HammerCharges = Math.Max(0, HammerCharges - 1);
                return;
            }

            RocketCharges = Math.Max(0, RocketCharges - 1);
        }
    }
}
