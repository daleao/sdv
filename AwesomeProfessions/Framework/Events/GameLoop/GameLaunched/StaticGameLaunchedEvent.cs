using JetBrains.Annotations;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Integrations;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.GameLaunched;

[UsedImplicitly]
internal class StaticGameLaunchedEvent : GameLaunchedEvent
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForAwesomeProfessions(
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

        // add Teh's Fishing Overhaul integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("TehPers.FishingOverhaul"))
            new TehsFishingOverhaulIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.Log, ModEntry.ModHelper)
                .Register();
    }
}