using System;
using AV.Framework.Core.Board;
using MessagePipe;

namespace AV.Framework.Application
{
    public sealed class GameFlow : IDisposable
    {
        private readonly ISubscriber<GoalReachedEvent> goalSubscriber;
        private readonly ISubscriber<PlayerDeathEvent> deathSubscriber;
        private IDisposable goalSubscription;
        private IDisposable deathSubscription;

        public GameFlowState State { get; private set; } = GameFlowState.MainMenu;
        public event Action<GameFlowState> StateChanged;

        public GameFlow(ISubscriber<GoalReachedEvent> goalSubscriber, ISubscriber<PlayerDeathEvent> deathSubscriber)
        {
            this.goalSubscriber = goalSubscriber ?? throw new ArgumentNullException(nameof(goalSubscriber));
            this.deathSubscriber = deathSubscriber ?? throw new ArgumentNullException(nameof(deathSubscriber));
        }

        public void Start()
        {
            goalSubscription = goalSubscriber.Subscribe(OnGoalReached);
            deathSubscription = deathSubscriber.Subscribe(OnPlayerDeath);
        }

        public void StartGame() => SetState(GameFlowState.Playing);

        public void WinGame()
        {
            if (State != GameFlowState.Playing) return;
            SetState(GameFlowState.Won);
        }

        public void LoseGame()
        {
            if (State != GameFlowState.Playing) return;
            SetState(GameFlowState.Lost);
        }

        public void ReturnToMenu() => SetState(GameFlowState.MainMenu);

        public void Dispose()
        {
            goalSubscription?.Dispose();
            deathSubscription?.Dispose();
        }

        private void OnGoalReached(GoalReachedEvent _) => WinGame();
        private void OnPlayerDeath(PlayerDeathEvent _) => LoseGame();

        private void SetState(GameFlowState state)
        {
            if (State == state) return;
            State = state;
            StateChanged?.Invoke(State);
        }
    }
}