namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        var registry = ModHelper.ModRegistry;

        // add Generic Mod Config Menu integration
        new Configs.GenericModConfigMenuIntegration(
            getConfig: () => Config,
            reset: () => Config = new ModConfig(),
            saveAndApply: () => ModHelper.WriteConfig(Config),
            modRegistry: registry,
            manifest: Manifest).Register();
    }
}
