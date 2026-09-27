namespace AV.Framework.Application.Audio
{
    using System;
    using AV.Framework.Application;
    using AV.Framework.Core.Board;
    using AV.Framework.Core.Events;
    using AV.Framework.Core.Gameplay;
    using AV.Framework.GameData;
    using MessagePipe;
    using UnityEngine;
    using VContainer.Unity;

    public sealed class GameAudioController : IStartable, IDisposable
    {
        private readonly GameAudioConfig audioConfig;
        private readonly GameFlow gameFlow;
        private readonly AudioSource audioSource;
        private readonly ISubscriber<PlayerMovedEvent> playerMovedSubscriber;
        private readonly ISubscriber<ObstacleCapturedEvent> obstacleCapturedSubscriber;
        private readonly ISubscriber<MoveUndoneEvent> moveUndoneSubscriber;
        private readonly ISubscriber<PowerUpUsedEvent> powerUpUsedSubscriber;
        private readonly ISubscriber<PowerUpCancelledEvent> powerUpCancelledSubscriber;
        private readonly ISubscriber<PlayerDeathEvent> playerDeathSubscriber;
        private readonly ISubscriber<GoalReachedEvent> goalReachedSubscriber;
        private IDisposable playerMovedSubscription;
        private IDisposable obstacleCapturedSubscription;
        private IDisposable moveUndoneSubscription;
        private IDisposable powerUpUsedSubscription;
        private IDisposable powerUpCancelledSubscription;
        private IDisposable playerDeathSubscription;
        private IDisposable goalReachedSubscription;
        public GameAudioController(
            GameAudioConfig audioConfig,
            GameFlow gameFlow,
            AudioSource audioSource,
            ISubscriber<PlayerMovedEvent> playerMovedSubscriber,
            ISubscriber<ObstacleCapturedEvent> obstacleCapturedSubscriber,
            ISubscriber<MoveUndoneEvent> moveUndoneSubscriber,
            ISubscriber<PowerUpUsedEvent> powerUpUsedSubscriber,
            ISubscriber<PowerUpCancelledEvent> powerUpCancelledSubscriber,
            ISubscriber<PlayerDeathEvent> playerDeathSubscriber,
            ISubscriber<GoalReachedEvent> goalReachedSubscriber)
        {
            this.audioConfig = audioConfig ?? throw new ArgumentNullException(nameof(audioConfig));
            this.gameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            this.audioSource = audioSource ?? throw new ArgumentNullException(nameof(audioSource));
            this.playerMovedSubscriber = playerMovedSubscriber ?? throw new ArgumentNullException(nameof(playerMovedSubscriber));
            this.obstacleCapturedSubscriber = obstacleCapturedSubscriber ?? throw new ArgumentNullException(nameof(obstacleCapturedSubscriber));
            this.moveUndoneSubscriber = moveUndoneSubscriber ?? throw new ArgumentNullException(nameof(moveUndoneSubscriber));
            this.powerUpUsedSubscriber = powerUpUsedSubscriber ?? throw new ArgumentNullException(nameof(powerUpUsedSubscriber));
            this.powerUpCancelledSubscriber = powerUpCancelledSubscriber ?? throw new ArgumentNullException(nameof(powerUpCancelledSubscriber));
            this.playerDeathSubscriber = playerDeathSubscriber ?? throw new ArgumentNullException(nameof(playerDeathSubscriber));
            this.goalReachedSubscriber = goalReachedSubscriber ?? throw new ArgumentNullException(nameof(goalReachedSubscriber));
        }

        public void Start()
        {
            playerMovedSubscription = playerMovedSubscriber.Subscribe(OnPlayerMoved);
            obstacleCapturedSubscription = obstacleCapturedSubscriber.Subscribe(OnObstacleCaptured);
            moveUndoneSubscription = moveUndoneSubscriber.Subscribe(OnMoveUndone);
            powerUpUsedSubscription = powerUpUsedSubscriber.Subscribe(OnPowerUpUsed);
            powerUpCancelledSubscription = powerUpCancelledSubscriber.Subscribe(OnPowerUpCancelled);
            playerDeathSubscription = playerDeathSubscriber.Subscribe(OnPlayerDeath);
            goalReachedSubscription = goalReachedSubscriber.Subscribe(OnGoalReached);
            gameFlow.StateChanged += OnGameFlowStateChanged;
        }

        public void Dispose()
        {
            gameFlow.StateChanged -= OnGameFlowStateChanged;
            playerMovedSubscription?.Dispose();
            obstacleCapturedSubscription?.Dispose();
            moveUndoneSubscription?.Dispose();
            powerUpUsedSubscription?.Dispose();
            powerUpCancelledSubscription?.Dispose();
            playerDeathSubscription?.Dispose();
            goalReachedSubscription?.Dispose();
        }

        private void OnPlayerMoved(PlayerMovedEvent playerMovedEvent)
        {
            Play(GameAudioType.PlayerMove);
        }

        private void OnObstacleCaptured(ObstacleCapturedEvent obstacleCapturedEvent)
        {
            Play(GameAudioType.ObstacleCapture);
        }

        private void OnMoveUndone(MoveUndoneEvent moveUndoneEvent)
        {
            Play(GameAudioType.Undo);
        }

        private void OnPowerUpUsed(PowerUpUsedEvent powerUpUsedEvent)
        {
            GameAudioType audioType = powerUpUsedEvent.PowerUpType == PowerUpType.Hammer
                ? GameAudioType.Hammer
                : GameAudioType.Rocket;

            Play(audioType);
        }

        private void OnPowerUpCancelled(PowerUpCancelledEvent powerUpCancelledEvent)
        {
            Play(GameAudioType.PowerUpCancel);
        }

        private void OnPlayerDeath(PlayerDeathEvent playerDeathEvent)
        {
            Play(GameAudioType.PlayerDeath);
        }

        private void OnGoalReached(GoalReachedEvent goalReachedEvent)
        {
            Play(GameAudioType.GoalReached);
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            if (state == GameFlowState.Won)
            {
                Play(GameAudioType.GameWon);
                return;
            }

            if (state == GameFlowState.Lost)
            {
                Play(GameAudioType.GameLost);
            }
        }

        private void Play(GameAudioType audioType)
        {
            if (!audioConfig.TryGetClip(audioType, out AudioClip clip)) return;
            audioSource.PlayOneShot(clip);
        }
    }
}
