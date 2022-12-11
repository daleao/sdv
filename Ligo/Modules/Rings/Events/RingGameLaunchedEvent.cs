namespace DaLion.Ligo.Modules.Rings.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class RingGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RingGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RingGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        var registry = ModHelper.ModRegistry;

        // add Garnet ring
        if (registry.IsLoaded("spacechase0.JsonAssets"))
        {
            new Integrations.JsonAssetsIntegration(registry).Register();
        }
        else
        {
            Log.W("Json Assets was not loaded. Features from the Rings module will not work correctly.");
        }

        // add Better Crafting integration
        if (registry.IsLoaded("leclair.bettercrafting"))
        {
            new Integrations.BetterCraftingIntegration(registry).Register();
        }
    }
}
