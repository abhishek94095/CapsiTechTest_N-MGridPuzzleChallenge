namespace AV.Framework.Application.Installers
{
    using MessagePipe;
    using VContainer;
    using VContainer.Unity;

    public static class ApplicationInstaller
    {
        public static void Install(IContainerBuilder builder)
        {
            builder.Register<BoardFactory>(Lifetime.Singleton);
            builder.Register<BoardPresenter>(Lifetime.Singleton);
            builder.Register<MoveHistory>(Lifetime.Singleton);
            builder.Register<GameFlow>(Lifetime.Singleton);
            builder.Register<GameSession>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SwipeInputController>();
            builder.RegisterEntryPoint<BoardInitializer>().AsSelf();
            builder.RegisterEntryPoint<PlayerMovement>();
            builder.RegisterEntryPoint<MovingPieceController>();
            builder.RegisterEntryPoint<GameFlowEntryPoint>();
            builder.RegisterMessagePipe();
        }
    }
}
