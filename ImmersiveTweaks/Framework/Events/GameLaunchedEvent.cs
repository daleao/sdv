using DaLion.Stardew.Common.Integrations;

namespace DaLion.Stardew.Tweaks.Framework.Events;

#region using directives

using StardewModdingAPI.Events;

using Integrations;

#endregion using directives

internal class GameLaunchedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.GameLaunched -= OnGameLaunched;
    }

    /// <summary>Raised after the game is launched, right before the first update tick.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    protected void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForImmersiveTweaks(
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
    }
}