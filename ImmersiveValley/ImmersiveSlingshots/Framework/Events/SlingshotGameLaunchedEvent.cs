namespace DaLion.Stardew.Slingshots.Framework.Events;

#region using directives

using DaLion.Common.Events;
using DaLion.Common.Extensions.SMAPI;
using DaLion.Stardew.Slingshots.Integrations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotGameLaunchedEvent(EventManager manager)
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
            new GenericModConfigMenuIntegrationForImmersiveSlingshots(
                getConfig: () => ModEntry.Config,
                reset: () =>
                {
                    ModEntry.Config = new ModConfig();
                    ModEntry.ModHelper.WriteConfig(ModEntry.Config);
                },
                saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
                modRegistry: registry,
                manifest: ModEntry.Manifest).Register();
        }

        // register new enchantments
        new SpaceCoreIntegration(registry).Register();

        // add Immersive Professions integration
        if (registry.IsLoaded("DaLion.ImmersiveProfessions"))
        {
            new ImmersiveProfessionsIntegration(registry).Register();
        }

        // add Immersive Arsenal integration
        if (registry.IsLoaded("DaLion.ImmersiveArsenal"))
        {
            ModEntry.ArsenalConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveArsenal");
        }

        if (ModEntry.Config.FaceMouseCursor)
        {
            ModEntry.Events.Enable<DriftButtonPressedEvent>();
        }
    }
}
