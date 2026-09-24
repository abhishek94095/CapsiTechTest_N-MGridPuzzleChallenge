namespace AV.Framework.Application.Installers
{
    using AV.Framework.Core.EventBus;
    using AV.Framework.Core.StateMachine;
    using VContainer;

    public static class CoreInstaller
    {
        public static void Install(IContainerBuilder builder)
        {
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);
        }
    }
}
