using VContainer.Unity;

namespace AV.Framework.Application
{
    public sealed class GameFlowEntryPoint : IStartable
    {
        private readonly GameFlow gameFlow;

        public GameFlowEntryPoint(GameFlow gameFlow)
        {
            this.gameFlow = gameFlow;
        }

        public void Start()
        {
            gameFlow.Start();
        }
    }
}