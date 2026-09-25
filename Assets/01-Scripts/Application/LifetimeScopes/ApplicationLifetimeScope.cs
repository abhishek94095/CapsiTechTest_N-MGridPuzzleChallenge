using AV.Framework.Application.Installers;
using AV.Framework.Core.Configuration;
using AV.Framework.GameData;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class ApplicationLifetimeScope : LifetimeScope
{
    [SerializeField] private EnvironmentSettingsSO environmentSettings;
    [SerializeField] private BoardData boardData;
    [SerializeField] private BoardVisualConfig boardVisualConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(environmentSettings);
        builder.RegisterInstance(boardData);
        builder.RegisterInstance(boardVisualConfig);

        if (environmentSettings != null && environmentSettings.SRDebuggerEnabled)
        {
            SRDebug.Init();
        }

        CoreInstaller.Install(builder);
        InfrastructureInstaller.Install(builder);
        ApplicationInstaller.Install(builder);
    }
}
