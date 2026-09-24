namespace AV.Framework.Core.Configuration
{
    public interface IEnvironmentService
    {
        EnvironmentType CurrentEnvironment { get; }

        bool IsDevelopment { get; }

        bool IsStaging { get; }

        bool IsProduction { get; }

        bool IsAnalyticsEnabled { get; }

        bool IsSRDebuggerEnabled { get; }
    }
}
