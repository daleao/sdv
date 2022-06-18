namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Extensions.Stardew;
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
        new GenericModConfigMenuIntegrationForImmersiveProfessions(
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

        // add SpaceCore integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            new SpaceCoreIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.Log).Register();

        // add Love Of Cooking integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            new LoveOfCookingIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.Log).Register();

        // add Luck Skill integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.LuckSkill"))
            new LuckSkillIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.Log).Register();

        // add Teh's Fishing Overhaul integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("TehPers.FishingOverhaul"))
            new TehsFishingOverhaulIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.Log, ModEntry.ModHelper)
                .Register();

        // add Custom Ore Nodes integration
        if (ModEntry.ModHelper.ModRegistry.IsLoaded("aedenthorn.CustomOreNodes"))
            new CustomOreNodesIntegration(ModEntry.ModHelper.ModRegistry, ModEntry.Log).Register();

        // add Immersive Suite integration
        ModEntry.ArsenalConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveArsenal", ModEntry.Log);
        ModEntry.PondsConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersivePonds", ModEntry.Log);
        ModEntry.RingsConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveRings", ModEntry.Log);
        ModEntry.TaxesConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveTaxes", ModEntry.Log);
        ModEntry.TweaksConfig = ModEntry.ModHelper.ReadConfigExt("DaLion.ImmersiveTweaks", ModEntry.Log);
        
        // add SVE integration
        ModEntry.SVEConfig = ModEntry.ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP", ModEntry.Log);

        // detect vintage interface
        if (ModEntry.Config.VintageInterfaceSupport == ModConfig.VintageInterfaceStyle.Automatic)
        {
            if (ModEntry.ModHelper.ModRegistry.IsLoaded("ManaKirel.VMI"))
                ModEntry.PlayerState.VintageInterface = "pink";
            else if (ModEntry.ModHelper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2"))
                ModEntry.PlayerState.VintageInterface = "brown";
            else
                ModEntry.PlayerState.VintageInterface = "off";
        }
        else
        {
            ModEntry.PlayerState.VintageInterface = ModEntry.Config.VintageInterfaceSupport.ToString().ToLowerInvariant();
        }
    }
}