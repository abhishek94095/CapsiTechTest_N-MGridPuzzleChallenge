using System.Collections.Generic;
using AV.Framework.Application.Installers;
using AV.Framework.Core.Configuration;
using AV.Framework.GameData;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class ApplicationLifetimeScope : LifetimeScope
{
    [SerializeField] private EnvironmentSettingsSO environmentSettings;
    [SerializeField] private List<BoardData> levels = new List<BoardData>();
    [SerializeField] private BoardVisualConfig boardVisualConfig;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private GameAudioConfig gameAudioConfig;
    [SerializeField] private AudioSource gameAudioSource;

    protected override void Configure(IContainerBuilder builder)
    {
        if (environmentSettings == null) throw new MissingReferenceException("Environment Settings is not assigned.");
        if (boardVisualConfig == null) throw new MissingReferenceException("Board Visual Config is not assigned.");
        if (gameCamera == null) throw new MissingReferenceException("Game Camera is not assigned.");
        if (gameAudioConfig == null) throw new MissingReferenceException("Game Audio Config is not assigned.");
        if (gameAudioSource == null) throw new MissingReferenceException("Game Audio Source is not assigned.");

        if (levels.Count == 0) throw new MissingReferenceException("At least one BoardData level is required.");

        builder.RegisterInstance(environmentSettings);
        builder.RegisterInstance(levels);
        builder.RegisterInstance(boardVisualConfig);
        builder.RegisterInstance(gameCamera);
        builder.RegisterInstance(gameAudioConfig);
        builder.RegisterInstance(gameAudioSource);

        if (environmentSettings.SRDebuggerEnabled)
        {
            SRDebug.Init();
        }

        CoreInstaller.Install(builder);
        InfrastructureInstaller.Install(builder);
        ApplicationInstaller.Install(builder);
    }
}
