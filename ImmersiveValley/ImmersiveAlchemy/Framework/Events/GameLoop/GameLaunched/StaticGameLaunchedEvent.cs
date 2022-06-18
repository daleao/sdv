namespace DaLion.Stardew.Alchemy.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Integrations;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Construct an instance.</summary>
    internal StaticGameLaunchedEvent()
    {
        Enable();
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForImmersiveAlchemy(
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
    }
}