namespace AV.Framework.Application.Installers
{
    using MessagePipe;
    using VContainer;

    public static class ApplicationInstaller
    {
        public static void Install(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
        }
    }
}
