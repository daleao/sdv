namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        var registry = ModHelper.ModRegistry;

        // register custom enchants
        new Integrations.SpaceCoreIntegration(registry).Register();

        // add custom items
        if (registry.IsLoaded("spacechase0.JsonAssets"))
        {
            new Integrations.JsonAssetsIntegration(registry).Register();
        }
        else
        {
            Log.W("Json Assets was not loaded. Features from the Arsenal module will be disabled.");
            ArsenalModule.Config.DwarvishCrafting = false;
            ArsenalModule.Config.InfinityPlusOne = false;
            ModHelper.WriteConfig(Config);
        }

        // add SVE integration
        if (registry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
        {
            new Integrations.StardewValleyExpandedIntegration(registry).Register();
        }
    }
}
