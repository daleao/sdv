namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using Integrations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalGameLaunchedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            new GenericModConfigMenuIntegrationForImmersiveArsenal(
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

        // add Hero Soul item
        new DynamicGameAssetsIntegration(ModEntry.ModHelper.ModRegistry).Register();
    }
}