using AV.Framework.Application.Installers;
using AV.Framework.Core.Configuration;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class ApplicationLifetimeScope : LifetimeScope
{
    [SerializeField] private EnvironmentSettingsSO environmentSettings;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(environmentSettings);

        if (environmentSettings != null && environmentSettings.SRDebuggerEnabled)
        {
            SRDebug.Init();
        }

        CoreInstaller.Install(builder);
        InfrastructureInstaller.Install(builder);
        ApplicationInstaller.Install(builder);
    }
}
