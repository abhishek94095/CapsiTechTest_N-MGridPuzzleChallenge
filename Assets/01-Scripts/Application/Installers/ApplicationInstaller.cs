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
            builder.RegisterEntryPoint<BoardInitializer>();
            builder.RegisterMessagePipe();
        }
    }
}
