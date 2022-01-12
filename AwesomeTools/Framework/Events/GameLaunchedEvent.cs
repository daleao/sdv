using StardewModdingAPI.Events;
using TheLion.Stardew.Tools.Integrations;

namespace TheLion.Stardew.Tools.Framework.Events;

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

    /// <summary>The event called after the first game update, once all mods are loaded.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForAwesomeTools(
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