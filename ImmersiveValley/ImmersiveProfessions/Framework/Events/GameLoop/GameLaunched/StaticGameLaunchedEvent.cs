namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Common.Extensions.SMAPI;
using Integrations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticGameLaunchedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        var registry = ModEntry.ModHelper.ModRegistry;

        // add Generic Mod Config Menu integration
        if (registry.IsLoaded("spacechase0.GenericModConfigMenu"))
            new GenericModConfigMenuIntegrationForImmersiveProfessions(
                getConfig: () => ModEntry.Config,
                reset: () =>
                {
                    ModEntry.Config = new();
                    ModEntry.ModHelper.WriteConfig(ModEntry.Config);
                },
                saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
                modRegistry: registry,
                manifest: ModEntry.Manifest
            ).Register();

        // add SpaceCore integration
        if (registry.IsLoaded("spacechase0.SpaceCore"))
            new SpaceCoreIntegration(registry).Register();

        // add Luck Skill integration
        if (registry.IsLoaded("spacechase0.LuckSkill"))
            new LuckSkillIntegration(registry).Register();

        // add Love Of Cooking integration
        if (registry.IsLoaded("blueberry.LoveOfCooking"))
            new LoveOfCookingIntegration(registry).Register();

        if (registry.IsLoaded("Pathoschild.Automate"))
            new AutomateIntegration(registry).Register(ModEntry.ModHelper);

        // add Teh's Fishing Overhaul integration
        if (registry.IsLoaded("TehPers.FishingOverhaul"))
            new TehsFishingOverhaulIntegration(registry, ModEntry.ModHelper.Events)
                .Register();

        // add Custom Ore Nodes integration
        if (registry.IsLoaded("aedenthorn.CustomOreNodes"))
            new CustomOreNodesIntegration(registry).Register();

        // add Immersive Suite integration
        if (registry.IsLoaded("DaLion.ImmersiveArsenal"))
            ModEntry.ArsenalConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveArsenal");
        if (registry.IsLoaded("DaLion.ImmersivePonds"))
            ModEntry.PondsConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersivePonds");
        if (registry.IsLoaded("DaLion.ImmersiveRings"))
            ModEntry.RingsConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveRings");
        if (registry.IsLoaded("DaLion.ImmersiveSlingshots"))
            ModEntry.SlingshotsConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveSlingshots");
        if (registry.IsLoaded("DaLion.ImmersiveTaxes"))
            ModEntry.TaxesConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveTaxes");
        if (registry.IsLoaded("DaLion.ImmersiveTweaks"))
            ModEntry.TweaksConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveTweaks");

        // add SVE integration
        ModEntry.SVEConfig = ModEntry.ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP");
    }
}