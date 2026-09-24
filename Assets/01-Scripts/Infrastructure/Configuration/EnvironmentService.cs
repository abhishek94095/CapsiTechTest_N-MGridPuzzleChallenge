using AV.Framework.Core.Configuration;

namespace AV.Framework.Infrastructure.Configuration
{
    public sealed class EnvironmentService : IEnvironmentService
    {
        private readonly EnvironmentSettingsSO _settings;

        public EnvironmentType CurrentEnvironment { get; }

        public bool IsDevelopment => CurrentEnvironment == EnvironmentType.Development;

        public bool IsStaging => CurrentEnvironment == EnvironmentType.Staging;

        public bool IsProduction => CurrentEnvironment == EnvironmentType.Production;

        public bool IsAnalyticsEnabled => _settings.AnalyticsEnabled;

        public bool IsSRDebuggerEnabled => _settings.SRDebuggerEnabled;

        public EnvironmentService(EnvironmentSettingsSO settings)
        {
            _settings = settings;

#if AV_DEVELOPMENT
            CurrentEnvironment = EnvironmentType.Development;
#elif AV_STAGING
            CurrentEnvironment = EnvironmentType.Staging;
#elif AV_PRODUCTION
            CurrentEnvironment = EnvironmentType.Production;
#else
            CurrentEnvironment = EnvironmentType.Development;
#endif
        }
    }
}
