namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Integrations;
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
        var registry = ModEntry.ModHelper.ModRegistry;

        // register custom enchants
        new SpaceCoreIntegration(registry).Register();

        // add custom items
        if (registry.IsLoaded("spacechase0.JsonAssets"))
        {
            new JsonAssetsIntegration(registry).Register();
        }
        else
        {
            Log.W("Json Assets was not loaded. Features from the Arsenal module will be disabled.");
            ModEntry.Config.Arsenal.AncientCrafting = false;
            ModEntry.Config.Arsenal.InfinityPlusOne = false;
            ModEntry.ModHelper.WriteConfig(ModEntry.Config);
        }
    }
}
