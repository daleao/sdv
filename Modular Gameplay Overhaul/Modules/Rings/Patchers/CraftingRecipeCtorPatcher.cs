namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CraftingRecipeCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CraftingRecipeCtorPatcher"/> class.</summary>
    internal CraftingRecipeCtorPatcher()
    {
        this.Target = this.RequireConstructor<CraftingRecipe>(typeof(string), typeof(bool));
    }

    #region harmony patches

    /// <summary>Fix localized display name for custom ring recipes.</summary>
    [HarmonyPostfix]
    private static void CraftingRecipeCtorPrefix(CraftingRecipe __instance, string name, bool isCookingRecipe)
    {
        if (isCookingRecipe || !__instance.name.Contains("Ring") || LocalizedContentManager.CurrentLanguageCode ==
            LocalizedContentManager.LanguageCode.en)
        {
            return;
        }

        __instance.DisplayName = name switch
        {
            "Glow Ring" => new Ring(ItemIDs.GlowRing).DisplayName,
            "Magnet Ring" => new Ring(ItemIDs.MagnetRing).DisplayName,
            "Emerald Ring" => new Ring(ItemIDs.EmeraldRing).DisplayName,
            "Aquamarine Ring" => new Ring(ItemIDs.AquamarineRing).DisplayName,
            "Ruby Ring" => new Ring(ItemIDs.RubyRing).DisplayName,
            "Amethyst Ring" => new Ring(ItemIDs.AmethystRing).DisplayName,
            "Topaz Ring" => new Ring(ItemIDs.TopazRing).DisplayName,
            "Jade Ring" => new Ring(ItemIDs.JadeRing).DisplayName,
            "Garnet Ring" => new Ring(Globals.GarnetRingIndex!.Value).DisplayName,
            _ => __instance.DisplayName,
        };
    }

    #endregion harmony patches
}
