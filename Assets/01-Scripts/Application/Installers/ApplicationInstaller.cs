namespace AV.Framework.Application.Installers
{
    using AV.Framework.Application;
    using AV.Framework.Core.Board;
    using AV.Framework.Core.Events;
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
            builder.RegisterComponentInHierarchy<PowerUpHud>();
            builder.RegisterComponentInHierarchy<GameHud>();
            builder.RegisterEntryPoint<SwipeInputController>();
            builder.RegisterEntryPoint<BoardInitializer>().AsSelf();
            builder.RegisterEntryPoint<PlayerMovement>();
            builder.RegisterEntryPoint<MovingPieceController>();
            builder.RegisterEntryPoint<PowerUpController>().AsSelf();
            builder.RegisterEntryPoint<GameFlowEntryPoint>();
            builder.RegisterMessagePipe();

            builder.RegisterBuildCallback(container =>
            {
                PowerUpHud powerUpHud = container.Resolve<PowerUpHud>();
                powerUpHud.Initialize(
                    container.Resolve<PowerUpController>(),
                    container.Resolve<ISubscriber<PowerUpModeStartedEvent>>(),
                    container.Resolve<ISubscriber<PowerUpModeEndedEvent>>());

                GameHud gameHud = container.Resolve<GameHud>();
                gameHud.Initialize(
                    container.Resolve<GameSession>(),
                    container.Resolve<IPublisher<UndoRequestedEvent>>());
            });
        }
    }
}
