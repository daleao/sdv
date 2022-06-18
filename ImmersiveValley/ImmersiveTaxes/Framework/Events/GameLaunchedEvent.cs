namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Integrations;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.GameLaunched"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal sealed class GameLaunchedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
        Log.D("[Taxes] Hooked GameLaunched event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.GameLaunched -= OnGameLaunched;
        Log.D("[Taxes] Unhooked GameLaunched event.");
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForImmersiveTaxes(
            getConfig: () => ModEntry.Config,
            reset: () =>
            {
                ModEntry.Config = new();
                ModEntry.ModHelper.WriteConfig(ModEntry.Config);
            },
            saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
            log: ModEntry.Log,
            modRegistry: ModEntry.ModHelper.ModRegistry,
            manifest: ModEntry.Manifest
        ).Register();

        // add Immersive Professions integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("DaLion.ImmersiveProfessions"))
            new ImmersiveProfessionsIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.Log).Register();
    }
}