using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AV.Framework.Core.Logging;

namespace AV.Framework.Core.StateMachine
{
    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly Dictionary<Type, IGameState> _states;
        private readonly ILogger _logger;

        public IGameState CurrentState { get; private set; }

        public GameStateMachine(IEnumerable<IGameState> states, ILogger logger)
        {
            _logger = logger;
            _states = states.ToDictionary(state => state.GetType(), state => state);
        }

        public async Task ChangeStateAsync<TState>() where TState : class, IGameState
        {
            if (!_states.TryGetValue(typeof(TState), out IGameState nextState))
            {
                _logger.LogError($"State '{typeof(TState).Name}' is not registered.");
                return;
            }

            if (ReferenceEquals(CurrentState, nextState))
            {
                return;
            }

            try
            {
                if (CurrentState != null)
                {
                    await CurrentState.ExitAsync();
                }

                CurrentState = nextState;
                await CurrentState.EnterAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError($"Exception while transitioning to '{typeof(TState).Name}'.\n{exception}");
            }
        }
    }
}
