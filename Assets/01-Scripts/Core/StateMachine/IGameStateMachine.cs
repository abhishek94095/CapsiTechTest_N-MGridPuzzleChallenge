using System.Threading.Tasks;

namespace AV.Framework.Core.StateMachine
{
    public interface IGameStateMachine
    {
        IGameState CurrentState { get; }

        Task ChangeStateAsync<TState>() where TState : class, IGameState;
    }
}
