﻿namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using Common.Events;
using Integrations;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PondGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondGameLaunchedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            new GenericModConfigMenuIntegrationForImmersivePonds(
                getConfig: () => ModEntry.Config,
                reset: () =>
                {
                    ModEntry.Config = new();
                    ModEntry.ModHelper.WriteConfig(ModEntry.Config);
                },
                saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
                modRegistry: ModEntry.ModHelper.ModRegistry,
                manifest: ModEntry.Manifest
            ).Register();

        // add Immersive Professions integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions"))
            new ImmersiveProfessionsIntegration(ModEntry.ModHelper.ModRegistry).Register();
    }
}