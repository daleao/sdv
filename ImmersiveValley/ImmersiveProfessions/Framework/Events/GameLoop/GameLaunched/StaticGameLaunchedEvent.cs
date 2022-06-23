namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;
using Common.Extensions.Stardew;
using Integrations;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticGameLaunchedEvent : GameLaunchedEvent
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object sender, GameLaunchedEventArgs e)
    {
        // add Generic Mod Config Menu integration
        new GenericModConfigMenuIntegrationForImmersiveProfessions(
            getConfig: () => ModEntry.Config,
            reset: () =>
            {
                ModEntry.Config = new();
                ModEntry.ModHelper.WriteConfig(ModEntry.Config);
            },
            saveAndApply: () => { ModEntry.ModHelper.WriteConfig(ModEntry.Config); },
            modRegistry: ModEntry.ModHelper.ModRegistry,
            manifest: ModEntry.Manifest
        ).Register();

        // add SpaceCore integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            new SpaceCoreIntegration(ModEntry.ModHelper.ModRegistry).Register();

        // add Love Of Cooking integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            new LoveOfCookingIntegration(ModEntry.ModHelper.ModRegistry).Register();

        // add Luck Skill integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.LuckSkill"))
            new LuckSkillIntegration(ModEntry.ModHelper.ModRegistry).Register();

        // add Teh's Fishing Overhaul integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("TehPers.FishingOverhaul"))
            new TehsFishingOverhaulIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.ModHelper)
                .Register();

        // add Custom Ore Nodes integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("aedenthorn.CustomOreNodes"))
            new CustomOreNodesIntegration(ModEntry.ModHelper.ModRegistry).Register();

        // add Immersive Suite integration
        ModEntry.ArsenalConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveArsenal");
        ModEntry.PondsConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersivePonds");
        ModEntry.RingsConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveRings");
        ModEntry.TaxesConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveTaxes");
        ModEntry.TweaksConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveTweaks");
        
        // add SVE integration
        ModEntry.SVEConfig = ModEntry.ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP");
    }
}