namespace DaLion.Ligo.Modules.Core.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
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
        var registry = ModEntry.ModHelper.ModRegistry;

        // add Generic Mod Config Menu integration
        Integrations.GmcmIntegration = new Configs.GenericModConfigMenuIntegration(
            getConfig: () => ModEntry.Config,
            reset: () => ModEntry.Config = new ModConfig(),
            saveAndApply: () =>
            {
                ModEntry.ModHelper.WriteConfig(ModEntry.Config);
                Integrations.GmcmIntegration.Reload();
            },
            modRegistry: registry,
            manifest: ModEntry.Manifest);

        Integrations.GmcmIntegration.Register();

        // add SVE integration
        if (registry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
        {
            Integrations.SveConfig = ModEntry.ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP");
        }
    }
}
