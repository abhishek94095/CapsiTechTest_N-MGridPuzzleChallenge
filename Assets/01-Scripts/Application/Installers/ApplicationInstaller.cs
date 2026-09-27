namespace AV.Framework.Application.Installers
{
    using AV.Framework.Application.Audio;
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
            builder.Register<BoardInitializer>(Lifetime.Singleton);
            builder.Register<LevelController>(Lifetime.Singleton);

            builder.RegisterEntryPoint<SwipeInputController>();
            builder.RegisterEntryPoint<PlayerMovement>();
            builder.RegisterEntryPoint<MovingPieceController>().AsSelf();
            builder.RegisterEntryPoint<PowerUpController>().AsSelf();
            builder.RegisterEntryPoint<GameFlowEntryPoint>();
            builder.RegisterEntryPoint<GameAudioController>();

            builder.RegisterComponentInHierarchy<MainMenuUI>();
            builder.RegisterComponentInHierarchy<GameplayHud>();
            builder.RegisterComponentInHierarchy<GameResultUI>();
            builder.RegisterComponentInHierarchy<PowerUpHud>();

            builder.RegisterMessagePipe();
        }
    }
}
