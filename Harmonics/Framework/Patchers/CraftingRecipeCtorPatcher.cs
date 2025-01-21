namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CraftingRecipeCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Fix localized display name for custom ring recipes.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CraftingRecipeCtorPrefix(CraftingRecipe __instance, string name, bool isCookingRecipe)
    {
        if (isCookingRecipe || !__instance.name.Contains("Ring") || LocalizedContentManager.CurrentLanguageCode ==
            LocalizedContentManager.LanguageCode.en || !Config.CraftableGemstoneRings)
        {
            return;
        }

        __instance.DisplayName = name switch
        {
            "Emerald Ring" => ItemRegistry.Create<Ring>(QIDs.EmeraldRing).DisplayName,
            "Aquamarine Ring" => ItemRegistry.Create<Ring>(QIDs.AquamarineRing).DisplayName,
            "Ruby Ring" => ItemRegistry.Create<Ring>(QIDs.RubyRing).DisplayName,
            "Amethyst Ring" => ItemRegistry.Create<Ring>(QIDs.AmethystRing).DisplayName,
            "Topaz Ring" => ItemRegistry.Create<Ring>(QIDs.TopazRing).DisplayName,
            "Jade Ring" => ItemRegistry.Create<Ring>(QIDs.JadeRing).DisplayName,
            "Garnet Ring" => ItemRegistry.Create<Ring>(GarnetRingId).DisplayName,
            _ => __instance.DisplayName,
        };
    }

    #endregion harmony patches
}
