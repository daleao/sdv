namespace DaLion.Stardew.Rings.Framework.Events;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Rings.Integrations;
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
        var registry = ModEntry.ModHelper.ModRegistry;

        // add Generic Mod Config Menu integration
        if (registry.IsLoaded("spacechase0.GenericModConfigMenu"))
        {
            new GenericModConfigMenuIntegrationForImmersiveRings(
                getConfig: () => ModEntry.Config,
                reset: () =>
                {
                    ModEntry.Config = new ModConfig();
                    ModEntry.ModHelper.WriteConfig(ModEntry.Config);
                },
                saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
                modRegistry: registry,
                manifest: ModEntry.Manifest)
                .Register();
        }

        // add Garnet ring
        if (registry.IsLoaded("spacechase0.JsonAssets"))
        {
            new JsonAssetsIntegration(registry).Register();
        }

        // add Better Crafting integration
        if (registry.IsLoaded("leclair.bettercrafting"))
        {
            new BetterCraftingIntegration(registry).Register();
        }
    }
}
