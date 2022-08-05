namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CraftingRecipeCtorPatch()
    {
        Target = RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Fix localized display name for custom ring recipes.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPrefix(CraftingRecipe __instance, string name, bool isCookingRecipe)
    {
        if (isCookingRecipe || !__instance.name.Contains("Ring") || LocalizedContentManager.CurrentLanguageCode ==
            LocalizedContentManager.LanguageCode.en) return;

        __instance.DisplayName = name switch
        {
            "Glow Ring" => new Ring(Constants.GLOW_RING_INDEX_I).DisplayName,
            "Magnet Ring" => new Ring(Constants.MAGNET_RING_INDEX_I).DisplayName,
            "Emerald Ring" => new Ring(Constants.EMERALD_RING_INDEX_I).DisplayName,
            "Aquamarine Ring" => new Ring(Constants.AQUAMARINE_RING_INDEX_I).DisplayName,
            "Ruby Ring" => new Ring(Constants.RUBY_RING_INDEX_I).DisplayName,
            "Amethyst Ring" => new Ring(Constants.AMETHYST_RING_INDEX_I).DisplayName,
            "Topaz Ring" => new Ring(Constants.TOPAZ_RING_INDEX_I).DisplayName,
            "Jade Ring" => new Ring(Constants.JADE_RING_INDEX_I).DisplayName,
            "Garnet Ring" => new Ring(ModEntry.GarnetRingIndex).DisplayName,
            _ => __instance.DisplayName
        };
    }

    #endregion harmony patches
}