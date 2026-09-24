using System.Threading.Tasks;

namespace AV.Framework.Core.StateMachine
{
    public interface IGameState
    {
        Task EnterAsync();

        Task ExitAsync();
    }
}
