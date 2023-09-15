namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

using DaLion.Overhaul.Modules.Combat.Integrations;

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
            "Emerald Ring" => new Ring(ObjectIds.EmeraldRing).DisplayName,
            "Aquamarine Ring" => new Ring(ObjectIds.AquamarineRing).DisplayName,
            "Ruby Ring" => new Ring(ObjectIds.RubyRing).DisplayName,
            "Amethyst Ring" => new Ring(ObjectIds.AmethystRing).DisplayName,
            "Topaz Ring" => new Ring(ObjectIds.TopazRing).DisplayName,
            "Jade Ring" => new Ring(ObjectIds.JadeRing).DisplayName,
            "Garnet Ring" => new Ring(JsonAssetsIntegration.GarnetRingIndex!.Value).DisplayName,
            _ => __instance.DisplayName,
        };
    }

    #endregion harmony patches
}
