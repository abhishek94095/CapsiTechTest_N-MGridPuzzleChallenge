namespace AV.Framework.Application.Installers
{
    using AV.Framework.Core.Configuration;
    using AV.Framework.Core.Logging;
    using AV.Framework.Infrastructure.Configuration;
    using AV.Framework.Infrastructure.Logging;
    using VContainer;

    public static class InfrastructureInstaller
    {
        public static void Install(IContainerBuilder builder)
        {
            builder.Register<IEnvironmentService, EnvironmentService>(Lifetime.Singleton);
            builder.Register<ILogger, UnityLogger>(Lifetime.Singleton);
        }
    }
}
