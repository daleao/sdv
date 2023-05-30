namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

#region using directives

using DaLion.Overhaul.Modules.Rings;
using DaLion.Overhaul.Modules.Rings.Integrations;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenu
{
    /// <summary>Register the config menu for RNGS.</summary>
    private void AddRingOptions()
    {
        this
            .AddPage(OverhaulModule.Rings.Namespace, I18n.Gmcm_Rngs_Heading)

            .AddCheckbox(
                () => "Rebalanced Rings",
                () => "Improves certain underwhelming rings.",
                config => config.Rings.RebalancedRings,
                (config, value) =>
                {
                    config.Rings.RebalancedRings = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                })
            .AddCheckbox(
                () => "Craftable Gemstone Rings",
                () => "Adds new combat recipes for crafting gemstone rings.",
                config => config.Rings.CraftableGemRings,
                (config, value) =>
                {
                    config.Rings.CraftableGemRings = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                })
            .AddCheckbox(
                () => "Better Glowstone Progression",
                () =>
                    "Replaces the glowstone ring recipe with one that makes sense, and adds complementary recipes for its constituents.",
                config => config.Rings.BetterGlowstoneProgression,
                (config, value) =>
                {
                    config.Rings.BetterGlowstoneProgression = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                })
            .SetTitleScreenOnlyForNextOptions(true)
            .AddCheckbox(
                () => "The One Infinity Band",
                () => "Replaces the Iridium Band recipe and effect. Adds new forge mechanics.",
                config => config.Rings.TheOneInfinityBand,
                (config, value) =>
                {
                    if (value && JsonAssetsIntegration.Instance?.IsLoaded != true)
                    {
                        Log.W(
                            "Cannot enable The One Iridium Band because this feature requires Json Assets which is not installed.");
                        return;
                    }

                    config.Rings.TheOneInfinityBand = value;
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Data/ObjectInformation");
                    ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
                    if (value && (JsonAssetsIntegration.Instance?.IsRegistered != true || !Globals.InfinityBandIndex.HasValue))
                    {
                        (JsonAssetsIntegration.Instance as IModIntegration)!.Register();
                    }
                })
            .SetTitleScreenOnlyForNextOptions(false)
            .AddCheckbox(
                () => "Enable Gemstone Resonance",
                () => "Allows gemstones to harmonize and resonate in close proximity of each other.",
                config => config.Rings.EnableResonance,
                (config, value) => config.Rings.EnableResonance = value)
            .AddCheckbox(
                () => "Colorful Resonance Glow",
                () => "Whether the glow light of resonating chords should take after the root note's color.",
                config => config.Rings.ColorfulResonance,
                (config, value) =>
                {
                    config.Rings.ColorfulResonance = value;
                    Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
                })
            .AddDropdown(
                () => "Resonance Light Source Texture",
                () => "The texture that should be used as the resonance light source.",
                config => config.Rings.LightsourceTexture.ToString(),
                (config, value) =>
                {
                    config.Rings.LightsourceTexture = Enum.Parse<Config.ResonanceLightsourceTexture>(value);
                    Game1.player.Get_ResonatingChords().ForEach(chord => chord.ResetLightSource());
                },
                new[] { "Sconce", "Stronger", "Patterned" },
                null);
    }
}
