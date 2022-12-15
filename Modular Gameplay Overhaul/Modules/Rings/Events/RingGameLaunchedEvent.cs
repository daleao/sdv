namespace DaLion.Overhaul.Modules.Rings.Events;

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

        // add Better Crafting integration
        if (registry.IsLoaded("leclair.bettercrafting"))
        {
            new Integrations.BetterCraftingIntegration(registry).Register();
        }

        // add Wear More Rings integration
        if (registry.IsLoaded("bcmpinc.WearMoreRings"))
        {
            new Integrations.WearMoreRingsIntegration(registry).Register();
        }

        // add Better Rings integration
        if (registry.IsLoaded("BBR.BetterRings"))
        {
            new Integrations.BetterRingsIntegration(registry).Register();
        }

        // add Vanilla Tweaks integration
        if (registry.IsLoaded("Taiyo.VanillaTweaks"))
        {
            new Integrations.VanillaTweaksIntegration(registry).Register();
        }

        // add Garnet Ring and Infinity Band
        if (registry.IsLoaded("spacechase0.JsonAssets"))
        {
            new Integrations.JsonAssetsIntegration(registry).Register();
        }
        else
        {
            Log.W("Json Assets was not loaded. Features from the Rings module will not work correctly.");
        }

    }
}
