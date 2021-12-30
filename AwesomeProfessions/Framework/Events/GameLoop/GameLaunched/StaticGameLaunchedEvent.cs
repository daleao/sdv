using JetBrains.Annotations;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Integrations;

namespace TheLion.Stardew.Professions.Framework.Events;

[UsedImplicitly]
internal class StaticGameLaunchedEvent : GameLaunchedEvent
{
    /// <inheritdoc />
    public override void OnGameLaunched(object sender, GameLaunchedEventArgs e)
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

        // add Teh's Fishing Overhaul integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("TehPers.FishingOverhaul"))
        {
            new TehsFishingOverhaulIntegration(modRegistry: ModEntry.ModHelper.ModRegistry, log: ModEntry.Log)
                .Register();
        }
    }
}